package cmd

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/wishlist"
	"github.com/boggydigital/vangogh/internal/mongocl"
)

func fetchWishlistPage(deps FetchDeps, page int) (totalPages int, err error) {
	wps, err := wishlist.Fetch(deps.HttpClient, deps.Media, false, page)
	if err != nil {
		return 1, err
	}

	for _, wp := range wps.Products {
		err = mongocl.Update(deps.MongoClient, deps.Ctx, deps.Collection, wp.ID, wp)
		if err != nil {
			return 1, err
		}
		fmt.Print(".")
	}

	return wps.TotalPages, nil
}

func FetchWishlist(deps FetchDeps) error {
	return fetchPages(deps, fetchWishlistPage)
}
