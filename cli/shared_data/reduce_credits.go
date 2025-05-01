package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"maps"
	"slices"
)

func reduceCredits(rdx redux.Writeable) error {

	rca := nod.Begin("reducing %s...", vangogh_integration.CreditsProperty)
	defer rca.Done()

	if err := rdx.MustHave(vangogh_integration.WikipediaRawProperties()...); err != nil {
		return err
	}

	idCredits := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.TitleProperty) {
		credits := make(map[string]any)

		for _, property := range vangogh_integration.WikipediaRawProperties() {
			if property == vangogh_integration.CreditsProperty {
				continue
			}

			if values, ok := rdx.GetAllValues(property, id); ok {
				for _, value := range values {
					credits[value] = nil
				}
			}
		}

		idCredits[id] = slices.Collect(maps.Keys(credits))
	}

	return rdx.BatchReplaceValues(vangogh_integration.CreditsProperty, idCredits)
}
