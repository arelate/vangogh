package shared_data

import (
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

const (
	idUltraPackDeluxeTurboHdRemixVol2 = "1207665493" // Ultra Pack Deluxe Turbo HD Remix vol.2
	idBuildCreator                    = "1838409417" // Build Creator

)

func reduceDeveloperPublisher(rdx redux.Writeable) error {

	ftdpa := nod.Begin(" reducing %s, %s...",
		vangogh_integration.GogDevelopersProperty,
		vangogh_integration.GogPublishersProperty)
	defer ftdpa.Done()

	fixedDevelopers := make(map[string][]string)
	fixedPublishers := make(map[string][]string)

	q := map[string][]string{vangogh_integration.GogDevelopersProperty: {"TEST DEVELOPER"}}

	for id := range rdx.Match(q) {

		if id == idUltraPackDeluxeTurboHdRemixVol2 {
			continue
		}

		if id == idBuildCreator {
			continue
		}

		var relatedGames []string

		if rgs, ok := rdx.GetAllValues(vangogh_integration.GogRequiresGamesProperty, id); ok && len(rgs) > 0 {
			relatedGames = rgs
		} else if iigs, sure := rdx.GetAllValues(vangogh_integration.GogIsIncludedByGamesProperty, id); sure && len(iigs) > 0 {
			relatedGames = iigs
		} else if igs, yeah := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, id); yeah {
			relatedGames = igs
		}

		for _, relatedId := range relatedGames {

			if dev, ok := rdx.GetLastVal(vangogh_integration.GogDevelopersProperty, relatedId); ok && !strings.HasPrefix(dev, "TEST DEVELOPER") {
				fixedDevelopers[id] = []string{dev}
			}

			if pub, ok := rdx.GetLastVal(vangogh_integration.GogPublishersProperty, relatedId); ok && !strings.HasPrefix(pub, "TEST PUBLISHER") {
				fixedPublishers[id] = []string{pub}
			}

			if len(fixedDevelopers[id]) > 0 && len(fixedPublishers[id]) > 0 {
				break
			}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.GogDevelopersProperty, fixedDevelopers); err != nil {
		return err
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.GogPublishersProperty, fixedPublishers); err != nil {
		return err
	}

	return nil
}
