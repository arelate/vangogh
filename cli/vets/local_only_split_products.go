package vets

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func LocalOnlySplitProducts(fix bool) error {

	sloa := nod.Begin("checking for local only split products...")
	defer sloa.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(vangogh_local_data.TitleProperty)
	if err != nil {
		return sloa.EndWithError(err)
	}

	for _, pagedPt := range vangogh_local_data.GOGPagedProducts() {

		splitPt := vangogh_local_data.SplitProductType(pagedPt)

		pa := nod.Begin(" checking %s not present in %s...", splitPt, pagedPt)

		localOnlyProducts, err := findLocalOnlySplitProducts(pagedPt)
		if err != nil {
			return pa.EndWithError(err)
		}

		if len(localOnlyProducts) > 0 {

			summary, err := vangogh_local_data.PropertyListsFromIdSet(
				localOnlyProducts,
				nil,
				[]string{vangogh_local_data.TitleProperty},
				rxa)

			if err != nil {
				_ = pa.EndWithError(err)
				continue
			}

			pa.EndWithSummary(fmt.Sprintf("found %d:", len(localOnlyProducts)), summary)

			if fix {
				fa := nod.Begin(" removing local only %s...", splitPt)
				if err := vangogh_local_data.Cut(maps.Keys(localOnlyProducts), splitPt); err != nil {
					return fa.EndWithError(err)
				}
				fa.EndWithResult("done")
			}
		} else {
			pa.EndWithResult("none found")
		}
	}

	sloa.EndWithResult("done")

	return nil
}
