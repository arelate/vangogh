package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

func Owned(ids map[string]bool) error {

	owned := make(map[string]bool, 0)

	properties := map[string]bool{
		vangogh_properties.TitleProperty:         true,
		vangogh_properties.IncludesGamesProperty: true,
	}

	exl, err := vangogh_extracts.NewListFromMap(properties)

	if err != nil {
		return err
	}

	vrLicenceProducts, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, gog_media.Game)

	if err != nil {
		return err
	}

	for id, ok := range ids {
		if !ok {
			continue
		}
		if vrLicenceProducts.Contains(id) {
			owned[id] = true
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
			owned[id] = true
		}
	}

	for id, ok := range ids {
		if !ok {
			continue
		}
		if owned[id] {
			fmt.Print("OWN:")
		} else {
			fmt.Print("NOT:")
		}
		printInfo(
			id,
			nil,
			map[string]bool{vangogh_properties.TitleProperty: true},
			exl)

	}

	return nil
}
