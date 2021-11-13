package checks

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api"
	"github.com/boggydigital/vangogh/cli_api/remove"
)

func LocalOnlySplitProducts(mt gog_media.Media, fix bool) error {

	sloa := nod.Begin("checking for local only split products...")
	defer sloa.End()

	for _, pagedPt := range vangogh_products.Paged() {

		splitPt := vangogh_products.SplitType(pagedPt)

		pa := nod.Begin(" checking %s not present in %s...", splitPt, pagedPt)

		localOnlyProducts, err := findLocalOnlySplitProducts(pagedPt, mt)
		if err != nil {
			return pa.EndWithError(err)
		}

		if localOnlyProducts.Len() > 0 {
			pa.EndWithResult("found %d", localOnlyProducts.Len())
			if err := cli_api.List(
				localOnlyProducts,
				0,
				splitPt,
				mt,
				nil); err != nil {
				return pa.EndWithError(err)
			}

			if fix {
				fa := nod.Begin(" removing local only %s...", splitPt)
				if err := remove.Data(localOnlyProducts.All(), splitPt, mt); err != nil {
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
