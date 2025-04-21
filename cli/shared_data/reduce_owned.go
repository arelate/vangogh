package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func reduceOwned(rdx redux.Writeable) error {

	roa := nod.Begin(" reducing %s...", vangogh_integration.OwnedProperty)
	defer roa.Done()

	owned := make(map[string][]string)

	// set all included products as owned
	for id := range rdx.Keys(vangogh_integration.LicencesProperty) {
		owned[id] = []string{vangogh_integration.TrueValue}
		if includesGames, ok := rdx.GetAllValues(vangogh_integration.IncludesGamesProperty, id); ok {
			for _, igId := range includesGames {
				owned[igId] = []string{vangogh_integration.TrueValue}
			}
		}
	}

	// set all PACKs as owned when all included products are owned
	for id := range rdx.Keys(vangogh_integration.IncludesGamesProperty) {
		if includesGames, ok := rdx.GetAllValues(vangogh_integration.IncludesGamesProperty, id); ok {
			includedGamesOwned := len(includesGames) > 0
			for _, igId := range includesGames {
				if !rdx.HasKey(vangogh_integration.LicencesProperty, igId) {
					includedGamesOwned = false
					break
				}
			}
			if includedGamesOwned {
				owned[id] = []string{vangogh_integration.TrueValue}
			}
		}
	}

	for id := range rdx.Keys(vangogh_integration.TitleProperty) {
		if _, ok := owned[id]; !ok {
			owned[id] = []string{vangogh_integration.FalseValue}
		}
	}

	// some products coming from licences will not have a title
	// we need to filter them out from owned, otherwise Search > Owned view
	// will have fewer products than declared (since product card are NOT
	// created for products without a title)
	for id := range owned {
		if !rdx.HasKey(vangogh_integration.TitleProperty, id) {
			owned[id] = []string{vangogh_integration.FalseValue}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.OwnedProperty, owned); err != nil {
		return err
	}

	return nil
}
