package cli

import (
	"fmt"
	"net/url"
	"sort"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func GetSummaryHandler(u *url.URL) error {
	return GetSummary()
}

func GetSummary() error {

	sa := nod.Begin("last sync summary:")
	defer sa.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.LastSyncUpdatesProperty,
		vangogh_local_data.TitleProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	summary := make(map[string][]string)

	for _, section := range rxa.Keys(vangogh_local_data.LastSyncUpdatesProperty) {
		ids, _ := rxa.GetAllUnchangedValues(vangogh_local_data.LastSyncUpdatesProperty, section)
		for _, id := range ids {
			if title, ok := rxa.GetFirstVal(vangogh_local_data.TitleProperty, id); ok {
				summary[section] = append(summary[section], fmt.Sprintf("%s %s", id, title))
			}
		}
	}

	sa.EndWithSummary("", summary)

	return nil
}

func humanReadable(productTypes map[vangogh_local_data.ProductType]bool) []string {
	hrStrings := make(map[string]bool, 0)
	for key, ok := range productTypes {
		if !ok {
			continue
		}
		hrStrings[key.HumanReadableString()] = true
	}

	keys := make([]string, 0, len(hrStrings))
	for key := range hrStrings {
		keys = append(keys, key)
	}

	sort.Strings(keys)

	return keys
}
