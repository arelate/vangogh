package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"sort"
	"strings"
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

func Summary(since int64, mt gog_media.Media) error {

	exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
	if err != nil {
		return err
	}

	created := make(map[string]map[vangogh_products.ProductType]bool, 0)
	modified := make(map[string]map[vangogh_products.ProductType]bool, 0)
	updated := make(map[string]bool, 0)

	for _, pt := range vangogh_products.Local() {

		if filterNewProductTypes[pt] {
			continue
		}

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		for _, id := range vr.CreatedAfter(since) {
			if created[id] == nil {
				created[id] = make(map[vangogh_products.ProductType]bool, 0)
			}
			created[id][pt] = true
			updated[id] = true
		}

		if filterUpdatedProductTypes[pt] {
			continue
		}

		for _, id := range vr.ModifiedAfter(since, true) {
			if created[id][pt] {
				continue
			}
			if modified[id] == nil {
				modified[id] = make(map[vangogh_products.ProductType]bool, 0)
			}
			updated[id] = true
			modified[id][pt] = true
		}
	}

	var msg string
	if len(updated) == 0 {
		msg = fmt.Sprintf("no new or updated products since %s.", time.Unix(since, 0).Format(time.Kitchen))
	} else {
		msg = fmt.Sprintf("key changes since %s:", time.Unix(since, 0).Format(time.Kitchen))
	}

	fmt.Println(msg)

	for id, _ := range updated {
		title, _ := exl.Get(vangogh_properties.TitleProperty, id)
		fmt.Println(id, title)
		if len(created[id]) > 0 {
			fmt.Println(" new in", strings.Join(humanReadable(created[id]), ","))
		}
		if len(modified[id]) > 0 {
			fmt.Println(" updated in", strings.Join(humanReadable(modified[id]), ","))
		}
	}

	return nil
}
