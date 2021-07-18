package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/internal"
	"net/http"
	"os"
	"path"
	"strings"
	"time"
)

type downloadListDelegate func(
	id string,
	dlList vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	forceRemoteUpdate bool) error

func GetDownloads(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_downloads.DownloadType,
	missing bool,
	modifiedSince int64,
	forceRemoteUpdate bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.LocalManualUrl)
	if err != nil {
		return err
	}

	if err := getDownloadsList(
		idSet,
		mt,
		exl,
		operatingSystems,
		langCodes,
		downloadTypes,
		downloadList,
		modifiedSince,
		forceRemoteUpdate); err != nil {
		return nil
	}

	return nil
}

func downloadManualUrl(
	slug string,
	dl *vangogh_downloads.Download,
	exl *vangogh_extracts.ExtractsList,
	httpClient *http.Client,
	dlClient *dolo.Client,
	forceRemoteUpdate bool) error {
	//downloading a manual URL is the following set of steps:
	//1 - check if local file exists (based on manualUrl -> relative localFile association) before attempting to resolve manualUrl
	//2 - resolve the source URL to an actual session URL
	//3 - construct local relative dir and filename based on manualUrl type (installer, movie, dlc, extra)
	//4 - for a given set of extensions - download validation file
	//5 - download authorized session URL to a file
	//6 - set association from manualUrl to a resolved filename
	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return err
	}

	fmt.Printf("downloading %s...", dl.String())

	//1
	if !forceRemoteUpdate {
		localFilename, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
		if ok {
			if _, err := os.Stat(path.Join(vangogh_urls.DownloadsDir(), localFilename)); !os.IsNotExist(err) {
				fmt.Println("already exists")
				return nil
			}
		}
	}

	//2
	resp, err := httpClient.Head(gog_urls.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		return err
	}
	resolvedUrl := resp.Request.URL
	if err := resp.Body.Close(); err != nil {
		return err
	}

	//3
	_, filename := path.Split(resolvedUrl.Path)
	relDir := dl.Dir(slug)
	absDir := path.Join(vangogh_urls.DownloadsDir(), relDir)

	//4
	remoteValidationPath := vangogh_urls.RemoteValidationPath(resolvedUrl.Path)
	if remoteValidationPath != "" {
		localValidationPath := vangogh_urls.LocalValidationPath(path.Join(absDir, filename))
		if _, err := os.Stat(localValidationPath); os.IsNotExist(err) {
			fmt.Print("xml")
			originalPath := resolvedUrl.Path
			resolvedUrl.Path = remoteValidationPath
			valDir, valFilename := path.Split(localValidationPath)
			if _, err := dlClient.Download(
				resolvedUrl, valDir, valFilename); err != nil {
				return err
			}
			resolvedUrl.Path = originalPath
			fmt.Print("...")
		}
	}

	//5
	if _, err := dlClient.Download(resolvedUrl, absDir, filename); err != nil {
		return err
	}

	//6
	if err := exl.Add(vangogh_properties.LocalManualUrl, dl.ManualUrl, path.Join(relDir, filename)); err != nil {
		return err
	}

	fmt.Println("done")
	return nil
}

func printCompletion(current, total uint64) {
	percent := (float32(current) / float32(total)) * 100
	if current == 0 {
		//we'll get the first notification before download starts and will output 4 spaces (XXX%)
		//that'll be deleted on updates
		fmt.Printf(strings.Repeat(" ", 4))
	}
	if current < total {
		//every update except the first (pre-download) and last (completion) are the same
		//move cursor 4 spaces back and print over current percent completion
		fmt.Printf("\x1b[4D%3.0f%%", percent)
	} else {
		//final update moves the cursor back 4 spaces to overwrite on the following updateß
		fmt.Printf("\x1b[4D")
	}
}

func downloadList(
	slug string,
	list vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	forceRemoteUpdate bool) error {
	fmt.Println("downloading", slug)

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	//there is no need to use internal httpClient with cookie support for downloading
	//manual downloads, so we're going to rely on default http.Client
	defaultClient := http.DefaultClient
	defaultClient.Timeout = time.Minute * 60
	dlClient := dolo.NewClient(defaultClient, printCompletion, dolo.Defaults())

	for _, dl := range list {
		if err := downloadManualUrl(slug, &dl, exl, httpClient, dlClient, forceRemoteUpdate); err != nil {
			fmt.Println(err)
		}
	}
	return nil
}

func getDownloadsList(
	idSet gost.StrSet,
	mt gog_media.Media,
	exl *vangogh_extracts.ExtractsList,
	operatingSystems []vangogh_downloads.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_downloads.DownloadType,
	delegate downloadListDelegate,
	modifiedSince int64,
	forceRemoteUpdate bool) error {

	if delegate == nil {
		return fmt.Errorf("vangogh: get downloads list delegate is nil")
	}
	if err := exl.AssertSupport(
		vangogh_properties.SlugProperty,
		vangogh_properties.NativeLanguageNameProperty); err != nil {
		return err
	}

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return err
	}

	for _, id := range idSet.All() {

		detSlug, ok := exl.Get(vangogh_properties.SlugProperty, id)

		if !vrDetails.Contains(id) || !ok {
			continue
		}

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		downloads, err := vangogh_downloads.FromDetails(det, mt, exl)
		if err != nil {
			return err
		}

		if !forceRemoteUpdate {
			forceRemoteUpdate = modifiedSince > 0 &&
				vrDetails.WasModifiedAfter(id, modifiedSince)
		}

		// already checked for nil earlier in the function
		if err := delegate(
			detSlug,
			downloads.Only(operatingSystems, langCodes, downloadTypes),
			exl,
			forceRemoteUpdate); err != nil {
			return err
		}
	}

	return nil
}
