package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

func Owned(ids ...string) error {

	owned := make(map[string]bool, 0)

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.IncludesGamesProperty)

	if err != nil {
		return err
	}

	vrLicenceProducts, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, gog_media.Game)

	if err != nil {
		return err
	}

	for _, id := range ids {
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

	for _, id := range ids {
		if owned[id] {
			fmt.Print("OWN:")
		} else {
			fmt.Print("GOG:")
		}
		printInfo(id, false, nil, []string{vangogh_properties.TitleProperty}, exl)

	}

	return nil
}
