package vets

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func LocalOnlySplitProducts(fix bool) error {

	sloa := nod.Begin("checking for local only split products...")
	defer sloa.End()

	rdx, err := vangogh_integration.NewReduxReader(vangogh_integration.TitleProperty)
	if err != nil {
		return sloa.EndWithError(err)
	}

	for _, pagedPt := range vangogh_integration.GOGPagedProducts() {

		splitPt := vangogh_integration.SplitProductType(pagedPt)

		pa := nod.Begin(" checking %s not present in %s...", splitPt, pagedPt)

		localOnlyProducts, err := findLocalOnlySplitProducts(pagedPt)
		if err != nil {
			return pa.EndWithError(err)
		}

		if len(localOnlyProducts) > 0 {

			summary, err := vangogh_integration.PropertyListsFromIdSet(
				maps.Keys(localOnlyProducts),
				nil,
				[]string{vangogh_integration.TitleProperty},
				rdx)

			if err != nil {
				_ = pa.EndWithError(err)
				continue
			}

			pa.EndWithSummary(fmt.Sprintf("found %d:", len(localOnlyProducts)), summary)

			if fix {
				fa := nod.Begin(" removing local only %s...", splitPt)

				kv, err := vangogh_integration.NewProductReader(splitPt)
				if err != nil {
					return fa.EndWithError(err)
				}
				for _, id := range maps.Keys(localOnlyProducts) {
					if err = kv.Cut(id); err != nil {
						return fa.EndWithError(err)
					}
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
