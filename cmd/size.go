package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
)

func Size(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_downloads.DownloadType) error {

	dlList := vangogh_downloads.DownloadsList{}

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty)
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
		func(
			_ string,
			list vangogh_downloads.DownloadsList,
			_ *vangogh_extracts.ExtractsList,
			_ bool) error {
			dlList = append(dlList, list...)
			return nil
		},
		0,
		false); err != nil {
		return err
	}

	fmt.Printf("estimated total download size: %.2fGB\n", dlList.TotalGBsEstimate())

	return nil
}
