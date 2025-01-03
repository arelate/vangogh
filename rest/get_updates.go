package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"golang.org/x/exp/maps"
	"net/http"
	"sort"
	"strconv"
	"time"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

const (
	updatedProductsLimit = 24 // divisible by 2,3,4,6
)

var sectionTitles = map[string]string{
	"new in store":       "Store additions",
	"new in account":     "Purchased recently",
	"new in wishlist":    "Wishlist additions",
	"released today":     "Today's releases",
	"updates in account": "Updated installers",
	"updates in news":    "Steam news",
}

func GetUpdates(w http.ResponseWriter, r *http.Request) {

	// GET /updates

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	showAll := r.URL.Query().Get("show-all") == "true"

	updates := make(map[string][]string)
	updateTotals := make(map[string]int)

	paginate := false

	for _, section := range rdx.Keys(vangogh_local_data.LastSyncUpdatesProperty) {

		ids, _ := rdx.GetAllValues(vangogh_local_data.LastSyncUpdatesProperty, section)
		updateTotals[section] = len(ids)
		// limit number of items only if there are at least x2 the limit
		// e.g. if the limit is 24, only start limiting if there are 49 or more items
		paginate = len(ids) > updatedProductsLimit*2
		for _, id := range ids {
			if paginate && !showAll && len(updates[section]) >= updatedProductsLimit {
				continue
			}
			updates[section] = append(updates[section], id)
		}
	}

	keys := make(map[string]bool)
	for _, ids := range updates {
		for _, id := range ids {
			keys[id] = true
		}
	}

	ids := maps.Keys(keys)
	sort.Strings(ids)

	updated := "recently"
	if scs, ok := rdx.GetLastVal(vangogh_local_data.SyncEventsProperty, vangogh_local_data.SyncCompleteKey); ok {
		if sci, err := strconv.ParseInt(scs, 10, 64); err == nil {
			updated = time.Unix(sci, 0).Format(time.RFC1123)
		}
	}

	// section order will be based on full title ("new in ...", "updates in ...")
	// so the order won't be changed after expanding titles
	sections := maps.Keys(updates)
	sort.Strings(sections)

	updatesPage := compton_pages.Updates(sections, updates, sectionTitles, updateTotals, updated, rdx)
	if err := updatesPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
