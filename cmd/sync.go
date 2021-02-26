package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/boggydigital/vangogh/internal"
)

func Sync(mt gog_types.Media) error {
	////sync paginated product types
	//productTypes := []vangogh_types.ProductType{vangogh_types.Store, vangogh_types.Account, vangogh_types.Wishlist}
	//for _, pt := range productTypes {
	//	if err := Fetch(nil, pt.String(), media, false); err != nil {
	//		return err
	//	}
	//}
	// sync main - detail missing product types
	productTypes := []vangogh_types.ProductType{vangogh_types.Details, vangogh_types.ApiProducts}
	for _, pt := range productTypes {
		var denyIds []string
		if pt == vangogh_types.ApiProducts {
			denyIds = internal.ReadLines("denylists/known-missing-api-products.txt")
		}
		if err := Fetch(nil, denyIds, pt, mt, true); err != nil {
			return err
		}
	}
	return nil
}
