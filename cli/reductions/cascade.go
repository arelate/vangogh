package reductions

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

var cascadingProperties = []string{
	vangogh_integration.GOGOrderDateProperty,
	vangogh_integration.GOGReleaseDateProperty,
	vangogh_integration.SteamAppIdProperty,
	vangogh_integration.VerticalImageProperty,
	vangogh_integration.RatingProperty,
}

// Cascade is a method to assign reductions to products that don't have them,
// and can get them through parent products. Current implementation is a
// template for additional properties and currently only cascades
// GOGOrderDateProperty from store-products (that are referenced in orders)
// to account-products that are linked as store-product.IncludesGames.
func Cascade() error {

	ca := nod.Begin("cascading supported properties...")
	defer ca.EndWithResult("done")

	rdx, err := vangogh_integration.NewReduxWriter(vangogh_integration.ReduxProperties()...)
	if err != nil {
		return err
	}

	if err := rdx.MustHave(vangogh_integration.IncludesGamesProperty); err != nil {
		return err
	}

	ids := rdx.Keys(vangogh_integration.IncludesGamesProperty)

	for id := range ids {
		includesIds, ok := rdx.GetAllValues(vangogh_integration.IncludesGamesProperty, id)
		if !ok {
			continue
		}
		for _, prop := range cascadingProperties {
			mainValues, ok := rdx.GetAllValues(prop, id)
			if !ok {
				continue
			}
			for _, includesId := range includesIds {
				if _, ok := rdx.GetAllValues(prop, includesId); !ok {
					if err := rdx.ReplaceValues(prop, includesId, mainValues...); err != nil {
						return err
					}
				}
			}
		}
	}

	return nil
}
