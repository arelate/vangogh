package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"strconv"
)

func Wishlisted() error {

	wa := nod.Begin(" %s...", vangogh_local_data.UserWishlist)
	defer wa.End()

	vrCatalogProducts, err := vangogh_local_data.NewReader(vangogh_local_data.CatalogProducts)
	if err != nil {
		return wa.EndWithError(err)
	}

	//using WishlistPage and not WishlistProduct for the remote source of truth
	vrUserWishlist, err := vangogh_local_data.NewReader(vangogh_local_data.UserWishlist)
	if err != nil {
		return wa.EndWithError(err)
	}

	wishlisted := map[string][]string{}

	//important to set all to false as a starting point to overwrite status
	//for product no longer wishlisted at the remote source of truth
	for _, id := range vrCatalogProducts.Keys() {
		wishlisted[id] = []string{"false"}
	}

	productsGetter, err := vrUserWishlist.ProductsGetter(vangogh_local_data.UserWishlist.String())
	for _, idGetter := range productsGetter.GetProducts() {
		id := strconv.Itoa(idGetter.GetId())
		wishlisted[id] = []string{"true"}
	}
	if err != nil {
		return wa.EndWithError(err)
	}

	wishlistedRdx, err := vangogh_local_data.ReduxWriter(vangogh_local_data.WishlistedProperty)
	if err != nil {
		return wa.EndWithError(err)
	}

	if err := wishlistedRdx.BatchReplaceValues(vangogh_local_data.WishlistedProperty, wishlisted); err != nil {
		return wa.EndWithError(err)
	}

	wa.EndWithResult("done")

	return nil
}
