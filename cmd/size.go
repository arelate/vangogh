package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
)

func Size(ids []string,
	slug string,
	mt gog_media.Media,
	osStrings []string,
	langCodes []string,
	dtStrings []string) error {

	dlList := vangogh_downloads.DownloadsList{}

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty)
	if err != nil {
		return err
	}

	if err := getDownloadsList(
		ids,
		slug,
		mt,
		exl,
		osStrings,
		langCodes,
		dtStrings,
		func(
			_ string,
			list vangogh_downloads.DownloadsList,
			_ *vangogh_extracts.ExtractsList) error {
			dlList = append(dlList, list...)
			return nil
		}); err != nil {
		return err
	}

	fmt.Printf("estimated total download size: %.2fGB\n", dlList.TotalGBsEstimate())

	return nil
}
