package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"os"
	"path/filepath"
)

func Cleanup(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_downloads.DownloadType,
	all bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.SlugProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.LocalManualUrl)
	if err != nil {
		return err
	}

	if all {
		vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
		if err != nil {
			return err
		}
		idSet.Add(vrDetails.All()...)
	}

	if err := mapDownloadsList(
		idSet,
		mt,
		exl,
		operatingSystems,
		langCodes,
		downloadTypes,
		cleanupDownloadList,
		0,
		false); err != nil {
		return err
	}

	return nil
}

func cleanupDownloadList(
	slug string,
	list vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	_ bool) error {
	fmt.Println("cleaning up", slug)

	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return err
	}

	//cleanup process:
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	expectedSet := gost.NewStrSet()

	for _, dl := range list {
		if localFilename, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl); ok {
			expectedSet.Add(localFilename)
		}
	}

	presentSet, err := vangogh_urls.LocalSlugDownloads(slug)
	if err != nil {
		return err
	}

	unexpectedFiles := presentSet.Except(expectedSet)
	if len(unexpectedFiles) == 0 {
		fmt.Printf("%s is already clean\n", slug)
		return nil
	}

	for _, unexpectedFile := range unexpectedFiles {
		downloadFilename := filepath.Join(vangogh_urls.DownloadsDir(), unexpectedFile)
		if _, err := os.Stat(downloadFilename); os.IsNotExist(err) {
			continue
		}
		fmt.Printf("removing %s...", downloadFilename)
		if err := os.Remove(downloadFilename); err != nil {
			return err
		}

		checksumFile := vangogh_urls.LocalValidationPath(unexpectedFile)
		if _, err := os.Stat(checksumFile); os.IsNotExist(err) {
			fmt.Println("done")
			continue
		}
		fmt.Print("xml...")
		if err := os.Remove(checksumFile); err != nil {
			return err
		}
		fmt.Println("done")
	}

	return nil
}
