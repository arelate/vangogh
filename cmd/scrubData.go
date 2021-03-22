package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"sort"
	"strconv"
)

func ScrubData(mt gog_media.Media, removeSurplus bool) error {
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

		sort.Strings(pagedIds)

		splitIds := make([]string, 0)

		splitPt := vangogh_products.SplitType(pagedPt)
		vrSplit, err := vangogh_values.NewReader(splitPt, mt)
		if err != nil {
			return err
		}
		for _, id := range vrSplit.All() {
			if sort.SearchStrings(pagedIds, id) == len(pagedIds) {
				splitIds = append(splitIds, id)
			}
		}

		if len(splitIds) > 0 {
			if removeSurplus {
				fmt.Printf("remove surplus %s (%s):\n", splitPt, mt)
				return removeData(splitIds, splitPt, mt)
			} else {
				fmt.Printf("%s not present in %s:\n", splitPt, pagedPt)
				return List(splitIds, splitPt, mt)
			}
		} else {
			fmt.Printf("%s and %s have all the same products\n", splitPt, pagedPt)
		}

	}
	return nil
}
