package cmd

import (
	"fmt"
	"github.com/arelate/gog_auth"
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
	"log"
	"net/http"
	"os"
	"path"
	"path/filepath"
	"strings"
)

type mapDownloadListDelegate func(
	id string,
	dlList vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	forceRemoteUpdate bool) error

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
	validate,
	noCleanup bool) error {

	httpClient, err := internal.HttpClient()
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
		vangogh_properties.LocalManualUrl)
	if err != nil {
		return err
	}

	if update {
		localSlugs, err := vangogh_urls.LocalSlugs()
		if err != nil {
			return err
		}
		localIds, err := SetFromSelectors(idSelectors{
			ids:       nil,
			slugs:     localSlugs,
			fromStdin: false,
		})
		idSet.AddSet(localIds)
	}

	if missing {
		if idSet.Len() > 0 {
			log.Printf("provided ids would be overwritten by 'missing' flag")
		}
		missingIds, err := idMissingLocalDownloads(mt, exl, operatingSystems, downloadTypes, langCodes)
		if err != nil {
			return err
		}
		idSet.AddSet(missingIds)
	}

	if err := mapDownloadsList(
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

	if !noCleanup {
		fmt.Println()
		if err := Cleanup(idSet, mt, operatingSystems, langCodes, downloadTypes, false); err != nil {
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
	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
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

	fmt.Printf("downloading %s...", dl.String())

	//2
	resp, err := httpClient.Head(gog_urls.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		fmt.Println()
		return err
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
	dlClient := dolo.NewClient(defaultClient, printCompletion, dolo.Defaults())

	for _, dl := range list {
		if err := downloadManualUrl(slug, &dl, exl, httpClient, dlClient, forceRemoteUpdate); err != nil {
			fmt.Println(err)
		}
	}
	return nil
}

func mapDownloadsList(
	idSet gost.StrSet,
	mt gog_media.Media,
	exl *vangogh_extracts.ExtractsList,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	mapDlDelegate mapDownloadListDelegate,
	modifiedSince int64,
	forceRemoteUpdate bool) error {

	if mapDlDelegate == nil {
		return fmt.Errorf("vangogh: get downloads list mapDlDelegate is nil")
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

	vrAccountProducts, err := vangogh_values.NewReader(vangogh_products.AccountProducts, mt)

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
				(vrDetails.WasModifiedAfter(id, modifiedSince) ||
					vrAccountProducts.WasModifiedAfter(id, modifiedSince))
		}

		// already checked for nil earlier in the function
		if err := mapDlDelegate(
			detSlug,
			downloads.Only(operatingSystems, downloadTypes, langCodes),
			exl,
			forceRemoteUpdate); err != nil {
			return err
		}
	}

	return nil
}

func idMissingLocalDownloads(
	mt gog_media.Media,
	exl *vangogh_extracts.ExtractsList,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string) (gost.StrSet, error) {
	//enumerating missing local downloads is a bit more complicated than images and videos
	//due to the fact that actual filenames are resolved when downloads are processed, so we can't compare
	//manualUrls and available files, we need to resolve manualUrls to actual local filenames first.
	//with this in mind we'll use different approach:
	//1. for all vangogh_products.Details ids:
	//2. check if there are unresolved manualUrls -> add to missingIds
	//3. check if slug dir is not present in downloads -> add to missingIds
	//4. check if any expected (resolved manualUrls) files are not present -> add to missingIds

	missingIds := gost.NewStrSet()

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return nil, err
	}

	// 1
	for _, id := range vrDetails.All() {

		det, err := vrDetails.Details(id)
		if err != nil {
			return missingIds, err
		}

		downloads, err := vangogh_downloads.FromDetails(det, mt, exl)
		if err != nil {
			return missingIds, err
		}

		downloads = downloads.Only(operatingSystems, downloadTypes, langCodes)

		expectedFiles := gost.NewStrSet()
		for _, dl := range downloads {
			file, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
			// 2
			if !ok || file == "" {
				missingIds.Add(id)
				break
			}
			expectedFiles.Add(file)
		}

		if expectedFiles.Len() == 0 {
			continue
		}

		slug, ok := exl.Get(vangogh_properties.SlugProperty, id)
		if !ok {
			continue
		}

		// 3
		if _, err := os.Stat(filepath.Join(vangogh_urls.DownloadsDir(), slug)); os.IsNotExist(err) {
			missingIds.Add(id)
			continue
		}

		presentFiles, err := vangogh_urls.LocalSlugDownloads(slug)
		if err != nil {
			return missingIds, nil
		}

		// 4
		missingFiles := expectedFiles.Except(presentFiles)
		if len(missingFiles) > 0 {
			missingIds.Add(id)
		}
	}

	return missingIds, nil
}
