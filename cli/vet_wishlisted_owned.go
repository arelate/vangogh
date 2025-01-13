package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func WishlistedOwned(fix bool) error {

	woa := nod.Begin("checking wishlisted owned products...")
	defer woa.End()

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.WishlistedProperty,
		vangogh_integration.OwnedProperty)

	if err != nil {
		return woa.EndWithError(err)
	}

	woid := make([]string, 0)

	for _, id := range rdx.Keys(vangogh_integration.WishlistedProperty) {
		// only check actually wishlisted products, not just any wishlisted state
		if rdx.HasValue(vangogh_integration.WishlistedProperty, id, vangogh_integration.FalseValue) {
			continue
		}
		if rdx.HasValue(vangogh_integration.OwnedProperty, id, vangogh_integration.TrueValue) {
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
