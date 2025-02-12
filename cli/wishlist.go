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
	defer wa.EndWithResult("done")

	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return err
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return err
	}

	if len(addProductIds) > 0 {
		if processedIds, err := wishlistAdd(addProductIds); err == nil {
			if err := gog_integration.AddToWishlist(hc, processedIds...); err != nil {
				return err
			}
		} else {
			return err

		}
	}

	if len(removeProductIds) > 0 {
		if processedIds, err := wishlistRemove(removeProductIds); err == nil {
			if err := gog_integration.RemoveFromWishlist(hc, processedIds...); err != nil {
				return err
			}
		} else {
			return err
		}
	}

	return nil
}

func wishlistAdd(
	ids []string) ([]string, error) {

	waa := nod.NewProgress(" adding product(s) to local wishlist...")
	defer waa.EndWithResult("done")

	pids, err := vangogh_integration.AddToLocalWishlist(ids, waa)
	if err != nil {
		return nil, err
	}

	return pids, err
}

func wishlistRemove(
	ids []string) ([]string, error) {

	wra := nod.NewProgress(" removing product(s) from local wishlist...")
	defer wra.EndWithResult("done")

	pids, err := vangogh_integration.RemoveFromLocalWishlist(ids, wra)
	if err != nil {
		return nil, err
	}

	return pids, err
}
