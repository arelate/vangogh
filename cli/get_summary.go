package cli

import (
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetSummaryHandler(_ *url.URL) error {
	return GetSummary()
}

func GetSummary() error {

	sa := nod.Begin("last sync summary:")
	defer sa.Done()

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.VangoghLastSyncUpdatesProperty,
		vangogh_integration.GogTitleProperty)
	if err != nil {
		return err
	}

	summary := make(map[string][]string)

	for section := range rdx.Keys(vangogh_integration.VangoghLastSyncUpdatesProperty) {
		ids, _ := rdx.GetAllValues(vangogh_integration.VangoghLastSyncUpdatesProperty, section)
		for _, id := range ids {
			if title, ok := rdx.GetLastVal(vangogh_integration.GogTitleProperty, id); ok {
				summary[section] = append(summary[section], id+" "+title)
			}
		}
	}

	sa.EndWithSummary("", summary)

	return nil
}
