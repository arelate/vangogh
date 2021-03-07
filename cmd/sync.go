package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/vangogh/internal"
)

func Sync(mt gog_types.Media) error {
	//sync paginated product types
	productTypes := []vangogh_types.ProductType{
		vangogh_types.StorePage,
		vangogh_types.AccountPage,
		vangogh_types.WishlistPage,
	}
	for _, pt := range productTypes {
		if err := Fetch(nil, nil, pt, mt, false); err != nil {
			return err
		}
	}

	//sync main - detail missing product types
	productTypes = []vangogh_types.ProductType{
		vangogh_types.Details,
		vangogh_types.ApiProductsV1,
		vangogh_types.ApiProductsV2,
	}
	for _, pt := range productTypes {
		denyIds := internal.ReadLines(vangogh_urls.DenylistUrl(pt))
		if err := Fetch(nil, denyIds, pt, mt, true); err != nil {
			return err
		}
	}

	// memorize properties
	productTypes = []vangogh_types.ProductType{
		vangogh_types.StoreProducts,
		vangogh_types.AccountProducts,
		vangogh_types.WishlistProducts,
		vangogh_types.Details,
		vangogh_types.ApiProductsV1,
		vangogh_types.ApiProductsV2,
	}
	properties := []string{
		vangogh_properties.TitleProperty,
		//vangogh_properties.DeveloperProperty,
		//vangogh_properties.PublisherProperty,
	}
	for _, pt := range productTypes {
		if err := Memorize(pt, mt, properties); err != nil {
			return err
		}
	}

	return nil
}
