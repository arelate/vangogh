package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"net/url"
	"slices"
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

	properties := slices.Collect(maps.Keys(propSet))

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir, properties...)
	if err != nil {
		return err
	}

	itp, err := vangogh_integration.PropertyListsFromIdSet(
		ids,
		nil,
		properties,
		rdx)

	if err != nil {
		return err
	}

	ia.EndWithSummary("", itp)

	return nil
}
