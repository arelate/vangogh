package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"strings"
	"time"
)

var filterNewProductTypes = map[vangogh_products.ProductType]bool{
	vangogh_products.Order: true,
	//vangogh_products.LicenceProducts: true,
}

var filterUpdatedProductTypes = map[vangogh_products.ProductType]bool{
	vangogh_products.StoreProducts:    true,
	vangogh_products.WishlistProducts: true,
	vangogh_products.ApiProductsV1:    true,
	vangogh_products.ApiProductsV2:    true,
}

func keys(someMap map[vangogh_products.ProductType]bool) []string {
	keys := make([]string, 0, len(someMap))
	for key, ok := range someMap {
		if !ok {
			continue
		}
		keys = append(keys, key.String())
	}
	return keys
}

func Summary(since int64, mt gog_media.Media) error {

	exl, err := vangogh_extracts.NewList(map[string]bool{vangogh_properties.TitleProperty: true})
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
			fmt.Println(" NEW:", strings.Join(keys(created[id]), ","))
		}
		if len(modified[id]) > 0 {
			fmt.Println(" UPD:", strings.Join(keys(modified[id]), ","))
		}
	}

	return nil
}
