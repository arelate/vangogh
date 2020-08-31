package cmd

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/products"
	"github.com/boggydigital/vangogh/internal/mongocl"
)

func fetchProductsPage(deps FetchDeps, page int) (totalPages int, err error) {
	ps, err := products.Fetch(deps.HttpClient, deps.Media, page)
	if err != nil {
		return 1, err
	}

	for _, p := range ps.Products {
		err = mongocl.Update(deps.MongoClient, deps.Ctx, deps.Collection, p.ID, p)
		if err != nil {
			return 1, err
		}
		fmt.Print(".")
	}

	return ps.TotalPages, nil
}

func FetchProducts(deps FetchDeps) error {
	return fetchPages(deps, fetchProductsPage)
}
