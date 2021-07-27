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

	if err := getDownloadsList(
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

	//cleanup procedure:
	//1. setup all expected files for a downloadList
	//2. setup all actual files for a slug (files present in a folder)
	//3. delete (actual files).Except(expected)

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

	fmt.Println(expectedSet)
	fmt.Println(presentSet)

	return nil
}
