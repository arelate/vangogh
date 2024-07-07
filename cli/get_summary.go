package cli

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
)

func GetSummaryHandler(u *url.URL) error {
	return GetSummary()
}

func GetSummary() error {

	sa := nod.Begin("last sync summary:")
	defer sa.End()

	rdx, err := vangogh_local_data.NewReduxReader(
		vangogh_local_data.LastSyncUpdatesProperty,
		vangogh_local_data.TitleProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	summary := make(map[string][]string)

	for _, section := range rdx.Keys(vangogh_local_data.LastSyncUpdatesProperty) {
		ids, _ := rdx.GetAllValues(vangogh_local_data.LastSyncUpdatesProperty, section)
		for _, id := range ids {
			if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
				summary[section] = append(summary[section], fmt.Sprintf("%s %s", id, title))
			}
		}
	}

	sa.EndWithSummary("", summary)

	return nil
}
