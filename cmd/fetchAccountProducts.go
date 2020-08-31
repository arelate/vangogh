package cmd

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/accountProducts"
	"github.com/boggydigital/vangogh/internal/mongocl"
)

func fetchAccountProductsPage(deps FetchDeps, page int) (totalPages int, err error) {
	aps, err := accountProducts.Fetch(deps.HttpClient, deps.Media, false, false, page)
	if err != nil {
		return 1, err
	}

	for _, ap := range aps.Products {
		err = mongocl.Update(deps.MongoClient, deps.Ctx, deps.Collection, ap.ID, ap)
		if err != nil {
			return 1, err
		}
		fmt.Print(".")
	}

	return aps.TotalPages, nil
}

func FetchAccountProducts(deps FetchDeps) error {
	return fetchPages(deps, fetchAccountProductsPage)
}
