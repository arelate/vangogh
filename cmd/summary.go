package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/vangogh/cmd/output"
	"sort"
	"time"
)

var filterNewProductTypes = map[vangogh_products.ProductType]bool{
	vangogh_products.Orders: true,
	//not all licence-products have associated api-products-v1/api-products-v2,
	//so in some cases we won't get a meaningful information like a title
	vangogh_products.LicenceProducts: true,
	//both ApiProductsVx are not interesting since they correspond to store-products or account-products
	vangogh_products.ApiProductsV1: true,
	vangogh_products.ApiProductsV2: true,
}

var filterUpdatedProductTypes = map[vangogh_products.ProductType]bool{
	//most of the updates are price changes for a sale, not that interesting for recurring sync
	vangogh_products.StoreProducts: true,
	// wishlist-products are basically store-products, so see above
	vangogh_products.WishlistProducts: true,
	//meaningful updates for account products come from details, not account-products
	vangogh_products.AccountProducts: true,
	//same as above for those product types
	vangogh_products.ApiProductsV1: true,
	vangogh_products.ApiProductsV2: true,
}

func Summary(since int64, mt gog_media.Media) error {

	updates := make(map[string][]string, 0)

	for _, pt := range vangogh_products.Local() {

		if filterNewProductTypes[pt] {
			continue
		}

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		categorize(vr.CreatedAfter(since),
			fmt.Sprintf("new in %s", pt.HumanReadableString()),
			updates)

		if filterUpdatedProductTypes[pt] {
			continue
		}

		categorize(vr.ModifiedAfter(since, true),
			fmt.Sprintf("updated in %s", pt.HumanReadableString()),
			updates)
	}

	if len(updates) == 0 {
		fmt.Printf("no new or updated products since %s.\n", time.Unix(since, 0).Format(time.Kitchen))
		return nil
	}

	fmt.Printf("key changes since %s:\n", time.Unix(since, 0).Format(time.Kitchen))

	return output.Groups(updates)
}

func humanReadable(productTypes map[vangogh_products.ProductType]bool) []string {
	hrStrings := make(map[string]bool, 0)
	for key, ok := range productTypes {
		if !ok {
			continue
		}
		hrStrings[key.HumanReadableString()] = true
	}

	keys := make([]string, 0, len(hrStrings))
	for key, _ := range hrStrings {
		keys = append(keys, key)
	}

	sort.Strings(keys)

	return keys
}

func categorize(ids []string, cat string, updates map[string][]string) {
	for _, id := range ids {
		if updates[cat] == nil {
			updates[cat] = make([]string, 0)
		}
		updates[cat] = append(updates[cat], id)
	}
}
