package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
)

func InfoHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Info(ids...)
}

func Info(ids ...string) error {

	ia := nod.Begin("information:")
	defer ia.Done()

	propSet := map[string]bool{vangogh_integration.TypesProperty: true}

	for _, p := range vangogh_integration.ReduxProperties() {
		propSet[p] = true
	}

	rdx, err := vangogh_integration.NewReduxReader(maps.Keys(propSet)...)
	if err != nil {
		return err
	}

	itp, err := vangogh_integration.PropertyListsFromIdSet(
		ids,
		nil,
		maps.Keys(propSet),
		rdx)

	if err != nil {
		return err
	}

	ia.EndWithSummary("", itp)

	return nil
}
