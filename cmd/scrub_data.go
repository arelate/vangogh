package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"strconv"
)

func ScrubData(mt gog_media.Media, fix bool) error {
	for _, pagedPt := range vangogh_products.AllPaged() {

		pagedIds := make([]string, 0)
		vrPaged, err := vangogh_values.NewReader(pagedPt, mt)
		if err != nil {
			return err
		}
		for _, id := range vrPaged.All() {
			productGetter, err := vrPaged.ProductGetter(id)
			if err != nil {
				return err
			}
			for _, idGetter := range productGetter.GetProducts() {
				pagedIds = append(pagedIds, strconv.Itoa(idGetter.GetId()))
			}
		}

		splitIds := make([]string, 0)

		splitPt := vangogh_products.SplitType(pagedPt)
		vrSplit, err := vangogh_values.NewReader(splitPt, mt)
		if err != nil {
			return err
		}

		for _, id := range vrSplit.All() {
			if stringsContain(pagedIds, id) {
				continue
			}
			splitIds = append(splitIds, id)
		}

		if len(splitIds) > 0 {
			if fix {
				fmt.Printf("fix %s (%s):\n", splitPt, mt)
				if err := removeData(splitIds, splitPt, mt); err != nil {
					return err
				}
			} else {
				fmt.Printf("%s not present in %s:\n", splitPt, pagedPt)
				if err := List(splitIds, 0, splitPt, mt); err != nil {
					return err
				}
			}
		} else {
			fmt.Printf("%s and %s have all the same products\n", splitPt, pagedPt)
		}
	}
	return nil
}

func stringsContain(all []string, s string) bool {
	for _, sa := range all {
		if sa == s {
			return true
		}
	}
	return false
}
