package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"strings"
)

const (
	idUltraPackDeluxeTurboHdRemixVol2 = "1207665493" // Ultra Pack Deluxe Turbo HD Remix vol.2
	idBuildCreator                    = "1838409417" // Build Creator

)

func reduceDeveloperPublisher(rdx redux.Writeable) error {

	ftdpa := nod.Begin(" reducing %s, %s...",
		vangogh_integration.DevelopersProperty,
		vangogh_integration.PublishersProperty)
	defer ftdpa.Done()

	fixedDevelopers := make(map[string][]string)
	fixedPublishers := make(map[string][]string)

	q := map[string][]string{vangogh_integration.DevelopersProperty: {"TEST DEVELOPER"}}

	for id := range rdx.Match(q) {

		if id == idUltraPackDeluxeTurboHdRemixVol2 {
			continue
		}

		if id == idBuildCreator {
			continue
		}

		var relatedGames []string

		if rgs, ok := rdx.GetAllValues(vangogh_integration.RequiresGamesProperty, id); ok && len(rgs) > 0 {
			relatedGames = rgs
		} else if iigs, sure := rdx.GetAllValues(vangogh_integration.IsIncludedByGamesProperty, id); sure && len(iigs) > 0 {
			relatedGames = iigs
		} else if igs, yeah := rdx.GetAllValues(vangogh_integration.IncludesGamesProperty, id); yeah {
			relatedGames = igs
		}

		for _, relatedId := range relatedGames {

			if dev, ok := rdx.GetLastVal(vangogh_integration.DevelopersProperty, relatedId); ok && !strings.HasPrefix(dev, "TEST DEVELOPER") {
				fixedDevelopers[id] = []string{dev}
			}

			if pub, ok := rdx.GetLastVal(vangogh_integration.PublishersProperty, relatedId); ok && !strings.HasPrefix(pub, "TEST PUBLISHER") {
				fixedPublishers[id] = []string{pub}
			}

			if len(fixedDevelopers[id]) > 0 && len(fixedPublishers[id]) > 0 {
				break
			}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.DevelopersProperty, fixedDevelopers); err != nil {
		return err
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.PublishersProperty, fixedPublishers); err != nil {
		return err
	}

	return nil
}
