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
	mt gog_media.Media,
	osStrings []string,
	langCodes []string,
	dtStrings []string,
	missing bool) error {

	if err := getDownloadsList(
		ids,
		slug,
		mt,
		osStrings,
		langCodes,
		dtStrings,
		missing,
		func(list vangogh_downloads.DownloadsList) {
			for _, dl := range list {
				fmt.Println(dl)
			}

		}); err != nil {
		return nil
	}

	////TODO: Write detailed plan for downloading files
	//
	return nil

}

func getDownloadsList(
	ids []string,
	slug string,
	mt gog_media.Media,
	osStrings []string,
	langCodes []string,
	dtStrings []string,
	missing bool,
	delegate func(list vangogh_downloads.DownloadsList)) error {

	operatingSystems := vangogh_downloads.ParseManyOperatingSystems(osStrings)
	downloadTypes := vangogh_downloads.ParseManyDownloadTypes(dtStrings)

	fmt.Printf("getting downloads for ids: %v operatingSystems: %v langCodes: %v downloadTypes: %v\n",
		ids,
		operatingSystems,
		langCodes,
		downloadTypes)

	idSet := gost.NewStrSetWith(ids...)

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty)
	if err != nil {
		return err
	}

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	if err != nil {
		return err
	}

	if slug != "" {
		slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
		idSet.Add(slugIds...)
	}

	for _, id := range idSet.All() {

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		downloads, err := vangogh_downloads.FromDetails(det, mt, exl)
		if err != nil {
			return err
		}

		delegate(downloads.Only(operatingSystems, langCodes, downloadTypes))
	}

	return nil
}
