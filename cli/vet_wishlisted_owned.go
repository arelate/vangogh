package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func WishlistedOwned(fix bool) error {

	woa := nod.Begin("checking wishlisted owned products...")
	defer woa.End()

	rdx, err := vangogh_local_data.ReduxReader(
		vangogh_local_data.WishlistedProperty,
		vangogh_local_data.OwnedProperty)

	if err != nil {
		return woa.EndWithError(err)
	}

	woid := make([]string, 0)

	for _, id := range rdx.Keys(vangogh_local_data.WishlistedProperty) {
		// only check actually wishlisted products, not just any wishlisted state
		if rdx.HasValue(vangogh_local_data.WishlistedProperty, id, vangogh_local_data.FalseValue) {
			continue
		}
		if rdx.HasValue(vangogh_local_data.OwnedProperty, id, vangogh_local_data.TrueValue) {
			woid = append(woid, id)
		}
	}

	if len(woid) > 0 {
		woa.EndWithResult("found %d product(s)", len(woid))

		if fix {
			rwoa := nod.Begin(" removing products from wishlist...")

			if err := Wishlist(nil, woid); err != nil {
				return rwoa.EndWithError(err)
			}

			rwoa.EndWithResult("done")
		}

	} else {
		woa.EndWithResult("all good")
	}

	return nil
}
