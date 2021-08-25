package cmd

import (
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/http_client"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/cmd/iterate"
	"github.com/boggydigital/vangogh/cmd/selectors"
	"log"
	"net/http"
	"os"
	"path"
	"strconv"
	"strings"
)

func GetDownloads(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	missing,
	update bool,
	modifiedSince int64,
	forceRemoteUpdate,
	validate bool) error {

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	li, err := gog_auth.LoggedIn(httpClient)
	if err != nil {
		return err
	}

	if !li {
		log.Fatalf("user is not logged in")
	}

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.LocalManualUrl,
		vangogh_properties.DownloadStatusError)
	if err != nil {
		return err
	}

	if update {
		localSlugs, err := vangogh_urls.LocalSlugs()
		if err != nil {
			return err
		}
		localIds, err := selectors.StrSetFrom(selectors.Id{
			Ids:       nil,
			Slugs:     localSlugs,
			FromStdin: false,
		})
		idSet.AddSet(localIds)
	}

	if missing {
		if idSet.Len() > 0 {
			log.Printf("provided ids would be overwritten by 'missing' flag")
		}
		missingIds, err := itemize.MissingLocalDownloads(mt, exl, operatingSystems, downloadTypes, langCodes)
		if err != nil {
			return err
		}

		if missingIds.Len() == 0 {
			fmt.Println("all downloads are available locally")
			return nil
		}

		idSet.AddSet(missingIds)
	}

	if err := iterate.DownloadsList(
		idSet,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		downloadList,
		modifiedSince,
		forceRemoteUpdate); err != nil {
		return nil
	}

	if validate {
		fmt.Println()
		if err := Validate(idSet, mt, operatingSystems, langCodes, downloadTypes, false); err != nil {
			return err
		}
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
	if err := exl.AssertSupport(
		vangogh_properties.LocalManualUrl,
		vangogh_properties.DownloadStatusError); err != nil {
		return err
	}

	//1
	if !forceRemoteUpdate {
		if localFilename, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl); ok {
			if _, err := os.Stat(path.Join(vangogh_urls.DownloadsDir(), localFilename)); !os.IsNotExist(err) {
				return nil
			}
		}
	}

	fmt.Printf(" %s...", dl.String())

	//2
	resp, err := httpClient.Head(gog_urls.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		fmt.Println()
		return err
	}
	//check for error status codes and store them for the manualUrl to provide a hint that locally missing file
	//is not a problem that can be solved locally (it's a remote source error)
	if resp.StatusCode > 299 {
		if err := exl.Set(vangogh_properties.DownloadStatusError, dl.ManualUrl, strconv.Itoa(resp.StatusCode)); err != nil {
			return err
		}
		return fmt.Errorf(resp.Status)
	}

	resolvedUrl := resp.Request.URL

	if err := resp.Body.Close(); err != nil {
		fmt.Println()
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
	if err := exl.Set(vangogh_properties.LocalManualUrl, dl.ManualUrl, path.Join(relDir, filename)); err != nil {
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
		//final update moves the cursor back 4 spaces to overwrite on the following update
		fmt.Printf("\x1b[4D")
	}
}

func downloadList(
	id string,
	slug string,
	list vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	forceRemoteUpdate bool) error {
	fmt.Println("downloading", slug)

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	//there is no need to use internal httpClient with cookie support for downloading
	//manual downloads, so we're going to rely on default http.Client
	defaultClient := http.DefaultClient
	dlClient := dolo.NewClient(defaultClient, printCompletion, dolo.Defaults())

	for _, dl := range list {
		if err := downloadManualUrl(slug, &dl, exl, httpClient, dlClient, forceRemoteUpdate); err != nil {
			fmt.Println(err)
		}
	}
	return nil
}
