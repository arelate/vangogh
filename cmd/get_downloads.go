package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
)

func GetDownloads(
	ids []string,
	slug string,
	osStrings []string,
	langCodes []string,
	dtStrings []string,
	missing bool) error {

	idSet := gost.NewStrSetWith(ids...)

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty)
	if err != nil {
		return err
	}

	operatingSystems := vangogh_downloads.ParseManyOperatingSystems(osStrings)
	downloadTypes := vangogh_downloads.ParseManyDownloadTypes(dtStrings)

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	if err != nil {
		return err
	}

	if slug != "" {
		slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
		idSet.Add(slugIds...)
	}

	fmt.Printf("getting downloads for ids: %v operatingSystems: %v langCodes: %v downloadTypes: %v\n",
		ids,
		operatingSystems,
		langCodes,
		downloadTypes)

	for _, id := range idSet.All() {

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		downloads, err := vangogh_downloads.FromDetails(det, exl)
		if err != nil {
			return err
		}

		downloads = downloads.Only(operatingSystems, langCodes, downloadTypes)

		fmt.Println("total estimated size:", downloads.TotalBytesEstimate())
		for _, dl := range downloads {
			fmt.Println(dl)
		}
	}

	//httpClient, err := internal.HttpClient()
	//if err != nil {
	//	return err
	//}

	//TODO: Write detailed plan for downloading files

	return nil

}
