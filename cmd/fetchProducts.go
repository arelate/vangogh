package cmd

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/products"
	"github.com/boggydigital/vangogh/internal/mongoclient"
	"go.mongodb.org/mongo-driver/mongo"
	"net/http"
)

func FetchProducts(httpClient *http.Client, mongoClient *mongo.Client, mt media.Type) error {

	ctx, cancel, err := mongocl.Connect(mongoClient)
	defer mongocl.Disconnect(ctx, cancel, mongoClient)

	if err != nil {
		return err
	}

	totalPages := 1

	for page := 1; page <= totalPages; page++ {

		fmt.Printf("Fetching page %3d of %3d", page, totalPages)
		prods, err := products.Fetch(httpClient, mt, page)

		if err != nil {
			return err
		}

		for _, prod := range prods.Products {
			err := mongocl.Update(mongoClient, ctx, mongocl.ProductsCollection, prod.ID, prod)
			if err != nil {
				return err
			}
			fmt.Print(".")
		}

		totalPages = prods.TotalPages
	}

	return nil
}
