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

func Summary(since int64, mt gog_media.Media) error {

	fmt.Printf("changes since %s:\n", time.Unix(since, 0).Format(time.Kitchen))

	exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
	if err != nil {
		return err
	}

	created := make(map[string][]string, 0)
	modified := make(map[string][]string, 0)
	updated := make([]string, 0)

	for _, pt := range vangogh_products.Local() {

		if pt == vangogh_products.Order {
			continue
		}

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		for _, id := range vr.CreatedAfter(since) {
			created[id] = append(created[id], pt.String())
			if !stringsContain(updated, id) {
				updated = append(updated, id)
			}
		}

		for _, id := range vr.ModifiedAfter(since, true) {
			if stringsContain(created[id], pt.String()) {
				continue
			}
			if !stringsContain(updated, id) {
				updated = append(updated, id)
			}
			modified[id] = append(modified[id], pt.String())
		}
	}

	for _, id := range updated {
		title, _ := exl.Get(vangogh_properties.TitleProperty, id)
		fmt.Println(id, title)
		if len(created[id]) > 0 {
			fmt.Println(" NEW:", strings.Join(created[id], ","))
		}
		if len(modified[id]) > 0 {
			fmt.Println(" UPDATED:", strings.Join(modified[id], ","))
		}
	}

	if len(updated) == 0 {
		fmt.Println(" found no new or updated products")
	}

	return nil
}
