package checks

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/nod"
)

func InvalidLocalProductData(mt gog_media.Media, fix bool) error {
	ilpa := nod.NewProgress("checking data for invalid products...")
	defer ilpa.End()

	invalidProducts := make(map[vangogh_products.ProductType][]string)

	allProductTypes := make(map[vangogh_products.ProductType]bool)
	for _, pt := range append(vangogh_products.Remote(), vangogh_products.Local()...) {
		allProductTypes[pt] = true
	}

	ilpa.TotalInt(len(allProductTypes))

	dataProblems := false

	for pt := range allProductTypes {

		if pt == vangogh_products.LicenceProducts {
			continue
		}

		invalidProducts[pt] = make([]string, 0)

		pta := nod.NewProgress(" checking %s...", pt)

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			_ = pta.EndWithError(err)
			continue
		}

		allProducts := vr.All()

		pta.TotalInt(len(allProducts))

		for _, id := range allProducts {
			prd, err := vr.ReadValue(id)
			if err != nil || prd == nil {
				invalidProducts[pt] = append(invalidProducts[pt], id)
				dataProblems = true
				if fix {
					if err := vr.Remove(id); err != nil {
						return err
					}
				}
			}
			pta.Increment()
		}

		pta.EndWithResult("done")
	}

	if !dataProblems {
		ilpa.EndWithResult("data seems ok")
	} else {
		exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
		if err != nil {
			return err
		}
		summary := make(map[string][]string)
		for pt, ids := range invalidProducts {
			if len(ids) == 0 {
				continue
			}
			ptStr := fmt.Sprintf("problems with %s:", pt)
			summary[ptStr] = make([]string, len(ids))
			for i := 0; i < len(ids); i++ {
				prodStr := ids[i]
				if title, ok := exl.Get(vangogh_properties.TitleProperty, ids[i]); ok {
					prodStr = fmt.Sprintf("%s %s", prodStr, title)
				}
				summary[ptStr][i] = prodStr
			}
		}
		ilpa.EndWithSummary(summary)
	}

	return nil
}
