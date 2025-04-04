package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"slices"
	"strconv"
)

func reduceTopTenPercent(rdx redux.Writeable) error {

	rttpa := nod.Begin(" reducing %s...", vangogh_integration.TopTenPercentProperty)
	defer rttpa.Done()

	topTenPercent := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.OpenCriticPercentileProperty) {

		if pcts, ok := rdx.GetLastVal(vangogh_integration.OpenCriticPercentileProperty, id); ok {
			if pcti, err := strconv.ParseInt(pcts, 10, 32); err == nil && pcti >= 90 {
				if pt, sure := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); sure && pt == "GAME" {
					if demo, yeah := rdx.GetLastVal(vangogh_integration.IsDemoProperty, id); yeah && demo != vangogh_integration.TrueValue {
						topTenPercent[id] = []string{vangogh_integration.TrueValue}
					}
				}
			}
		}

	}

	currentTopTenPercent := slices.Collect(rdx.Keys(vangogh_integration.TopTenPercentProperty))
	if err := rdx.CutKeys(vangogh_integration.TopTenPercentProperty, currentTopTenPercent...); err != nil {
		return err
	}

	return rdx.BatchAddValues(vangogh_integration.TopTenPercentProperty, topTenPercent)
}
