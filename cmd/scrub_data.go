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

		pagedIds := make([]string, 0)
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
			fmt.Printf("%s not present in %s:\n", splitPt, pagedPt)
			if err := List(splitIds, 0, splitPt, mt); err != nil {
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
	//propExtracts, err := vangogh_properties.PropExtracts(vangogh_properties.AllExtracted())
	//if err != nil {
	//	return err
	//}

	//for _, pt := range vangogh_products.AllLocal() {
	//
	//	supProps := vangogh_properties.Supported(pt, vangogh_properties.AllExtracted())
	//
	//	vr, err := vangogh_values.NewReader(pt, mt)
	//	if err != nil {
	//		return err
	//	}

	//for _, id := range vr.All() {
	//propValues, err := vangogh_properties.GetProperties(id, vr, supProps)
	//if err != nil {
	//	return err
	//}
	//
	//for _, prop := range supProps {
	//
	//	if prop == vangogh_properties.ScreenshotsProperty {
	//		continue
	//	}

	//extract, _ := propExtracts[prop].Get(id)
	//if propValues[prop] != "" && propValues[prop] != extract {
	//	fmt.Println(pt, mt, id, prop, "extract:", extract, "propValue:", propValues[prop])
	//}
	//}

	//}

	//}

	// fmt.Println("images that are not linked to a product:")

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
