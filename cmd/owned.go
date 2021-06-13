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

func Owned(ids []string) error {

	ownedSet := gost.NewStrSet()

	idSet := gost.StrSetWith(ids...)

	propSet := gost.StrSetWith(
		vangogh_properties.TitleProperty,
		vangogh_properties.IncludesGamesProperty)

	exl, err := vangogh_extracts.NewList(propSet.All()...)

	if err != nil {
		return err
	}

	vrLicenceProducts, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, gog_media.Game)

	if err != nil {
		return err
	}

	for _, id := range idSet.All() {

		if vrLicenceProducts.Contains(id) {
			ownedSet.Add(id)
			continue
		}

		includesGames, ok := exl.GetAllRaw(vangogh_properties.IncludesGamesProperty, id)
		if !ok {
			continue
		}

		ownAllIncludedGames := true
		for _, igId := range includesGames {
			ownAllIncludedGames = ownAllIncludedGames && vrLicenceProducts.Contains(igId)
			if !ownAllIncludedGames {
				break
			}
		}

		if ownAllIncludedGames {
			ownedSet.Add(id)
		}
	}

	for _, id := range idSet.All() {
		if ownedSet.Has(id) {
			fmt.Print("OWN:")
		} else {
			fmt.Print("NOT:")
		}
		if err := printInfo(
			id,
			nil,
			[]string{vangogh_properties.TitleProperty},
			exl); err != nil {
			return err
		}

	}

	return nil
}
