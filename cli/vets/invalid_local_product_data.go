package vets

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func InvalidLocalProductData(fix bool) error {
	ilpa := nod.NewProgress("checking data for invalid products...")
	defer ilpa.End()

	invalidProducts := make(map[vangogh_local_data.ProductType][]string)

	allProductTypes := make(map[vangogh_local_data.ProductType]bool)

	pts := vangogh_local_data.GOGRemoteProducts()
	pts = append(pts, vangogh_local_data.SteamRemoteProducts()...)
	pts = append(pts, vangogh_local_data.LocalProducts()...)

	for _, pt := range pts {
		allProductTypes[pt] = true
	}

	ilpa.TotalInt(len(allProductTypes))

	dataProblems := false

	for pt := range allProductTypes {

		if pt == vangogh_local_data.LicenceProducts {
			continue
		}

		invalidProducts[pt] = make([]string, 0)

		pta := nod.NewProgress(" checking %s...", pt)

		vr, err := vangogh_local_data.NewProductReader(pt)
		if err != nil {
			_ = pta.EndWithError(err)
			continue
		}

		allProducts, err := vr.Keys()
		if err != nil {
			_ = pta.EndWithError(err)
			continue
		}

		pta.TotalInt(len(allProducts))

		for _, id := range allProducts {
			prd, err := vr.ReadValue(id)
			if err != nil || prd == nil {
				invalidProducts[pt] = append(invalidProducts[pt], id)
				dataProblems = true
				if fix {
					if _, err := vr.Cut(id); err != nil {
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
		rdx, err := vangogh_local_data.NewReduxReader(vangogh_local_data.TitleProperty)
		if err != nil {
			return err
		}
		summary := make(map[string][]string)
		for pt, ids := range invalidProducts {
			if len(ids) == 0 {
				continue
			}
			ptStr := fmt.Sprintf("%s:", pt)
			summary[ptStr] = make([]string, len(ids))
			for i := 0; i < len(ids); i++ {
				prodStr := ids[i]
				if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, ids[i]); ok {
					prodStr = fmt.Sprintf("%s %s", prodStr, title)
				}
				summary[ptStr][i] = prodStr
			}
		}

		heading := "found problems:"
		if fix {
			heading = "fixing problems:"
		}
		ilpa.EndWithSummary(heading, summary)
	}

	return nil
}
