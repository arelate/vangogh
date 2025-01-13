package cli

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
	"net/url"
)

func WishlistHandler(u *url.URL) error {
	return Wishlist(
		vangogh_integration.ValuesFromUrl(u, "add"),
		vangogh_integration.ValuesFromUrl(u, "remove"))
}

func Wishlist(addProductIds, removeProductIds []string) error {

	wa := nod.Begin("performing requested wishlist operations...")
	defer wa.End()

	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return wa.EndWithError(err)
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return wa.EndWithError(err)
	}

	if len(addProductIds) > 0 {
		if processedIds, err := wishlistAdd(addProductIds); err == nil {
			if err := gog_integration.AddToWishlist(hc, processedIds...); err != nil {
				return wa.EndWithError(err)
			}
		} else {
			return wa.EndWithError(err)

		}
	}

	if len(removeProductIds) > 0 {
		if processedIds, err := wishlistRemove(removeProductIds); err == nil {
			if err := gog_integration.RemoveFromWishlist(hc, processedIds...); err != nil {
				return wa.EndWithError(err)
			}
		} else {
			return wa.EndWithError(err)
		}
	}

	wa.EndWithResult("done")

	return nil
}

func wishlistAdd(
	ids []string) ([]string, error) {

	waa := nod.NewProgress(" adding product(s) to local wishlist...")
	defer waa.End()

	pids, err := vangogh_integration.AddToLocalWishlist(ids, waa)
	if err != nil {
		waa.EndWithError(err)
	} else {
		waa.EndWithResult("done")
	}

	return pids, err
}

func wishlistRemove(
	ids []string) ([]string, error) {

	wra := nod.NewProgress(" removing product(s) from local wishlist...")
	defer wra.End()

	pids, err := vangogh_integration.RemoveFromLocalWishlist(ids, wra)
	if err != nil {
		wra.EndWithError(err)
	} else {
		wra.EndWithResult("done")
	}

	return pids, err
}
