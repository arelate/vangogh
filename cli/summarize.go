package cli

import (
	"net/url"
	"time"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func SummarizeHandler(u *url.URL) error {

	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return err
	}

	return Summarize(
		since)
}

func Summarize(since int64) error {

	sa := nod.Begin("summarizing updates...")
	defer sa.End()

	updates, err := vangogh_local_data.Updates(since)
	if err != nil {
		return sa.EndWithError(err)
	}

	if len(updates) == 0 {
		return nil
	}

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.LastSyncUpdatesProperty,
		vangogh_local_data.TitleProperty,
		vangogh_local_data.GOGReleaseDateProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	summary := make(map[string][]string)

	//set new values for each section
	for section, ids := range updates {
		sortedIds, err := rxa.Sort(maps.Keys(ids),
			vangogh_local_data.DefaultDesc,
			vangogh_local_data.DefaultSort)
		if err != nil {
			return sa.EndWithError(err)
		}
		summary[section] = sortedIds
	}

	//clean sections filled earlier that don't exist anymore
	for _, section := range rxa.Keys(vangogh_local_data.LastSyncUpdatesProperty) {
		if _, ok := updates[section]; ok {
			continue
		}
		summary[section] = nil
	}

	if rt, err := releasedToday(rxa); err == nil {
		if len(rt) > 0 {
			summary["released today"] = rt
		}
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.LastSyncUpdatesProperty, summary); err != nil {
		sa.EndWithError(err)
	}

	sa.EndWithResult("done")

	return nil
}

func releasedToday(rxa kvas.ReduxAssets) ([]string, error) {

	if err := rxa.IsSupported(vangogh_local_data.GOGReleaseDateProperty); err != nil {
		return nil, err
	}

	ids := make([]string, 0)
	today := time.Now().Format("2006.01.02")

	for _, id := range rxa.Keys(vangogh_local_data.GOGReleaseDateProperty) {
		if rt, ok := rxa.GetFirstVal(vangogh_local_data.GOGReleaseDateProperty, id); ok {
			if rt == today {
				ids = append(ids, id)
			}
		}
	}

	return rxa.Sort(ids, vangogh_local_data.DefaultDesc, vangogh_local_data.DefaultSort)
}
