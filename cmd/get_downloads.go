package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
)

const (
	windows = "windows"
	macos   = "macos"
	linux   = "linux"
)

func GetDownloads(ids []string, os []string, langCodes []string, all bool) error {
	fmt.Printf("get %s, %s downloads for %v\n", os, langCodes, ids)

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	if err != nil {
		return err
	}

	exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty,
		vangogh_properties.NativeLanguageNameProperty)
	if err != nil {
		return err
	}

	langNames := gost.NewStrSet()
	for _, lc := range langCodes {
		langName, ok := exl.Get(vangogh_properties.NativeLanguageNameProperty, lc)
		if !ok || langName == "" {
			continue
		}
		langNames.Add(langName)
	}

	osSet := gost.StrSetWith(os...)

	for _, id := range ids {

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
			if !langNames.Has(dl.Language) {
				continue
			}
			fmt.Println(dl.Language)
			if osSet.Has(windows) && len(dl.Windows) > 0 {
				fmt.Println(" Windows:", dl.Windows)
			}
			if osSet.Has(macos) && len(dl.Mac) > 0 {
				fmt.Println(" Mac:", dl.Mac)
			}
			if osSet.Has(linux) && len(dl.Linux) > 0 {
				fmt.Println(" Linux:", dl.Linux)
			}
		}
	}

	return nil
}
