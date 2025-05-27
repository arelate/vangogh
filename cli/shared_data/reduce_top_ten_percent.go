package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"slices"
	"strconv"
)

func reduceTopPercent(rdx redux.Writeable) error {

	rttpa := nod.Begin(" reducing %s...", vangogh_integration.TopPercentProperty)
	defer rttpa.Done()

	topPercents := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.OpenCriticPercentileProperty) {

		if pt, sure := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); sure && pt == vangogh_integration.GameProductType {
			if demo, yeah := rdx.GetLastVal(vangogh_integration.IsDemoProperty, id); yeah && demo != vangogh_integration.TrueValue {

				if pcts, ok := rdx.GetLastVal(vangogh_integration.OpenCriticPercentileProperty, id); ok {
					if pcti, err := strconv.ParseInt(pcts, 10, 32); err == nil {
						switch {
						case pcti == 99:
							topPercents[id] = []string{"1%"}
						case pcti >= 95:
							topPercents[id] = []string{"5%"}
						case pcti >= 90:
							topPercents[id] = []string{"10%"}
						case pcti >= 80:
							topPercents[id] = []string{"20%"}
						}
					}
				}
			}
		}

	}

	currentTopPercents := slices.Collect(rdx.Keys(vangogh_integration.TopPercentProperty))
	if err := rdx.CutKeys(vangogh_integration.TopPercentProperty, currentTopPercents...); err != nil {
		return err
	}

	return rdx.BatchAddValues(vangogh_integration.TopPercentProperty, topPercents)
}
