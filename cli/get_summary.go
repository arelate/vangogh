package cli

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/url"
)

func GetSummaryHandler(u *url.URL) error {
	return GetSummary()
}

func GetSummary() error {

	sa := nod.Begin("last sync summary:")
	defer sa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir,
		vangogh_integration.LastSyncUpdatesProperty,
		vangogh_integration.TitleProperty)
	if err != nil {
		return err
	}

	summary := make(map[string][]string)

	for section := range rdx.Keys(vangogh_integration.LastSyncUpdatesProperty) {
		ids, _ := rdx.GetAllValues(vangogh_integration.LastSyncUpdatesProperty, section)
		for _, id := range ids {
			if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
				summary[section] = append(summary[section], fmt.Sprintf("%s %s", id, title))
			}
		}
	}

	sa.EndWithSummary("", summary)

	return nil
}
