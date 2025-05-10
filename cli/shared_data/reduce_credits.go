package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"maps"
	"slices"
)

func reduceCredits(rdx redux.Writeable) error {

	rca := nod.Begin(" reducing %s...", vangogh_integration.CreditsProperty)
	defer rca.Done()

	if err := rdx.MustHave(vangogh_integration.CreditsProperties()...); err != nil {
		return err
	}

	idCredits := make(map[string][]string)

	authorCredits := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.TitleProperty) {
		credits := make(map[string]any)

		for _, property := range vangogh_integration.CreditsProperties() {

			if property == vangogh_integration.CreditsProperty ||
				property == vangogh_integration.HasMultipleCreditsProperty {
				continue
			}

			if values, ok := rdx.GetAllValues(property, id); ok {
				for _, value := range values {
					if len(authorCredits[value]) == 0 {
						authorCredits[value] = []string{vangogh_integration.FalseValue}
					} else if len(authorCredits[value]) == 1 &&
						authorCredits[value][0] == vangogh_integration.FalseValue {
						authorCredits[value] = []string{vangogh_integration.TrueValue}
					}
					credits[value] = nil
				}
			}
		}

		idCredits[id] = slices.Collect(maps.Keys(credits))
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.HasMultipleCreditsProperty, authorCredits); err != nil {
		return err
	}

	return rdx.BatchReplaceValues(vangogh_integration.CreditsProperty, idCredits)
}
