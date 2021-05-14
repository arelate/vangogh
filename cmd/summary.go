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

var filterProductTypeChanges = map[vangogh_products.ProductType]bool{
	vangogh_products.Order:            true,
	vangogh_products.StoreProducts:    true,
	vangogh_products.WishlistProducts: true,
	vangogh_products.ApiProductsV1:    true,
	vangogh_products.ApiProductsV2:    true,
}

func Summary(since int64, mt gog_media.Media) error {

	exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
	if err != nil {
		return err
	}

	created := make(map[string][]string, 0)
	modified := make(map[string][]string, 0)
	updated := make(map[string]bool, 0)

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		for _, id := range vr.CreatedAfter(since) {
			created[id] = append(created[id], pt.String())
			updated[id] = true
		}

		if filterProductTypeChanges[pt] {
			continue
		}

		for _, id := range vr.ModifiedAfter(since, true) {
			if stringsContain(created[id], pt.String()) {
				continue
			}
			updated[id] = true
			modified[id] = append(modified[id], pt.String())
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
			fmt.Println(" NEW:", strings.Join(created[id], ","))
		}
		if len(modified[id]) > 0 {
			fmt.Println(" UPD:", strings.Join(modified[id], ","))
		}
	}

	return nil
}
