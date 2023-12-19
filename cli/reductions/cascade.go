package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

var cascadingProperties = []string{
	vangogh_local_data.GOGOrderDateProperty,
	vangogh_local_data.GOGReleaseDateProperty,
	vangogh_local_data.SteamAppIdProperty,
	vangogh_local_data.VerticalImageProperty,
	vangogh_local_data.RatingProperty,
}

// Cascade is a method to assign reductions to products that don't have them,
// and can get them through parent products. Current implementation is a
// template for additional properties and currently only cascades
// GOGOrderDateProperty from store-products (that are referenced in orders)
// to account-products that are linked as store-product.IncludesGames.
func Cascade() error {

	ca := nod.NewProgress("cascading supported properties...")
	defer ca.End()

	rdx, err := vangogh_local_data.ReduxWriter(vangogh_local_data.ReduxProperties()...)
	if err != nil {
		return ca.EndWithError(err)
	}

	if err := rdx.MustHave(vangogh_local_data.IncludesGamesProperty); err != nil {
		return ca.EndWithError(err)
	}

	ids := rdx.Keys(vangogh_local_data.IncludesGamesProperty)

	ca.TotalInt(len(ids))

	for _, id := range ids {
		includesIds, ok := rdx.GetAllValues(vangogh_local_data.IncludesGamesProperty, id)
		if !ok {
			ca.Increment()
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
						return ca.EndWithError(err)
					}
				}
			}
		}
		ca.Increment()
	}

	return nil
}
