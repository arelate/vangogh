package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"strconv"
)

func ScrubData(mt gog_media.Media, fix bool) error {

	fmt.Println("split products missing from the paged data:")
	for _, pagedPt := range vangogh_products.Paged() {

		pagedIds := make(map[string]bool, 0)

		vrPaged, err := vangogh_values.NewReader(pagedPt, mt)
		if err != nil {
			return err
		}
		for _, id := range vrPaged.All() {
			productGetter, err := vrPaged.ProductsGetter(id)
			if err != nil {
				return err
			}
			for _, idGetter := range productGetter.GetProducts() {
				pagedIds[strconv.Itoa(idGetter.GetId())] = true
			}
		}

		splitIds := make(map[string]bool, 0)

		splitPt := vangogh_products.SplitType(pagedPt)
		vrSplit, err := vangogh_values.NewReader(splitPt, mt)
		if err != nil {
			return err
		}

		for _, id := range vrSplit.All() {
			if pagedIds[id] {
				continue
			}
			splitIds[id] = true
		}

		if len(splitIds) > 0 {
			fmt.Printf("%s not present in %s:\n", splitPt, pagedPt)
			if err := List(splitIds, 0, splitPt, mt, nil); err != nil {
				return err
			}

			if fix {
				fmt.Printf("fix %s (%s):\n", splitPt, mt)
				if err := removeData(splitIds, splitPt, mt); err != nil {
					return err
				}
			}
		} else {
			fmt.Printf("%s and %s have all the same products\n", splitPt, pagedPt)
		}
	}

	// fmt.Println("products with values different from extracts:")
	// fmt.Println("images that are not linked to a product:")

	return nil
}
