package reductions

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"strconv"
)

func Wishlisted() error {

	wa := nod.Begin(" %s...", vangogh_integration.UserWishlist)
	defer wa.End()

	vrCatalogProducts, err := vangogh_integration.NewProductReader(vangogh_integration.CatalogProducts)
	if err != nil {
		return wa.EndWithError(err)
	}

	//using WishlistPage and not WishlistProduct for the remote source of truth
	vrUserWishlist, err := vangogh_integration.NewProductReader(vangogh_integration.UserWishlist)
	if err != nil {
		return wa.EndWithError(err)
	}

	wishlisted := map[string][]string{}

	//important to set all to false as a starting point to overwrite status
	//for product no longer wishlisted at the remote source of truth

	for id := range vrCatalogProducts.Keys() {
		wishlisted[id] = []string{"false"}
	}

	productsGetter, err := vrUserWishlist.ProductsGetter(vangogh_integration.UserWishlist.String())
	for _, idGetter := range productsGetter.GetProducts() {
		id := strconv.Itoa(idGetter.GetId())
		wishlisted[id] = []string{"true"}
	}
	if err != nil {
		return wa.EndWithError(err)
	}

	wishlistedRdx, err := vangogh_integration.NewReduxWriter(vangogh_integration.WishlistedProperty)
	if err != nil {
		return wa.EndWithError(err)
	}

	if err := wishlistedRdx.BatchReplaceValues(vangogh_integration.WishlistedProperty, wishlisted); err != nil {
		return wa.EndWithError(err)
	}

	wa.EndWithResult("done")

	return nil
}
