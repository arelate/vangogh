package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"maps"
	"net/http"
	"slices"
	"strconv"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

const (
	updatedProductsLimit = 10 // divisible by 2,3,4,5,6
)

func GetUpdates(w http.ResponseWriter, r *http.Request) {

	// GET /updates?section

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	showAll := q.Get("show-all") == "true"
	section := q.Get("section")

	updates := make(map[string][]string)
	updateTotals := make(map[string]int)

	paginate := false

	for updateSection := range rdx.Keys(vangogh_integration.LastSyncUpdatesProperty) {

		ids, _ := rdx.GetAllValues(vangogh_integration.LastSyncUpdatesProperty, updateSection)
		updateTotals[updateSection] = len(ids)
		// limit number of items only if there are at least x2 the limit
		// e.g. if the limit is 24, only start limiting if there are 49 or more items
		paginate = len(ids) > updatedProductsLimit*2
		for _, id := range ids {
			if paginate && !showAll && len(updates[updateSection]) >= updatedProductsLimit {
				continue
			}
			updates[updateSection] = append(updates[updateSection], id)
		}
	}

	keys := make(map[string]bool)
	for _, ids := range updates {
		for _, id := range ids {
			keys[id] = true
		}
	}

	updated := "recently"
	if scs, ok := rdx.GetLastVal(vangogh_integration.SyncEventsProperty, vangogh_integration.SyncCompleteKey); ok {
		if sci, err := strconv.ParseInt(scs, 10, 64); err == nil {
			updated = time.Unix(sci, 0).Format(time.RFC1123)
		}
	}

	if section == "" {
		if sortedSections := slices.Sorted(maps.Keys(updates)); len(sortedSections) > 0 {
			http.Redirect(w, r, "/updates?section="+sortedSections[0], http.StatusTemporaryRedirect)
			return
		}
	}

	updatesPage := compton_pages.Updates(section, updates, updateTotals, updated, rdx)
	if err := updatesPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
