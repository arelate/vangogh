package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

func GetDownloads(ids map[string]bool, os []string, lang []string, all bool) error {
	fmt.Printf("get %s, %s downloads for %v\n", os, lang, ids)

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	if err != nil {
		return err
	}

	exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty,
		vangogh_properties.LanguageNameProperty)
	if err != nil {
		return err
	}

	langCodes := exl.Search(map[string][]string{vangogh_properties.LanguageNameProperty: lang}, true)
	fmt.Println(langCodes)

	for id, ok := range ids {
		if !ok {
			continue
		}

		title, _ := exl.Get(vangogh_properties.TitleProperty, id)
		fmt.Println(id, title)

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		downloads, err := det.GetDownloads()
		if err != nil {
			return err
		}

		for _, dl := range downloads {
			fmt.Println(dl.Language)
			if len(dl.Windows) > 0 {
				fmt.Println(" Windows:", dl.Windows)
			}
			if len(dl.Mac) > 0 {
				fmt.Println(" Mac:", dl.Mac)
			}
			if len(dl.Linux) > 0 {
				fmt.Println(" Linux:", dl.Linux)
			}
		}
	}

	return nil
}
