package cmd

import "github.com/arelate/vangogh_types"

func Sync(media string) error {
	//sync paginated product types
	productTypes := []vangogh_types.ProductType{vangogh_types.Store, vangogh_types.Account, vangogh_types.Wishlist}
	for _, pt := range productTypes {
		if err := Fetch(nil, pt.String(), media, false); err != nil {
			return err
		}
	}
	// sync main - detail missing product types
	productTypes = []vangogh_types.ProductType{vangogh_types.Details, vangogh_types.ApiProducts}
	for _, pt := range productTypes {
		if err := Fetch(nil, pt.String(), media, true); err != nil {
			return err
		}
	}
	return nil
}
