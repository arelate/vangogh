package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/http"
)

func Search(w http.ResponseWriter, r *http.Request) {

	// GET /search?text&(searchable properties)&sort&desc&format

	query := make(map[string][]string)
	q := r.URL.Query()

	for _, p := range vangogh_local_data.SearchableProperties() {
		if q.Has(p) {
			vals := q[p]
			if len(vals) == 0 ||
				(len(vals) == 1 && vals[0] == "") {
				continue
			}
			query[p] = vals
		}
	}

	sort := q.Get(vangogh_local_data.SortProperty)
	if sort == "" {
		sort = vangogh_local_data.TitleProperty
	}
	desc := q.Get(vangogh_local_data.DescendingProperty) == "true"

	properties := []string{sort}
	for p := range query {
		properties = append(properties, p)
	}

	//detailedProperties := vangogh_local_data.DetailAllAggregateProperties(properties...)

	if err := RefreshReduxAssets(properties...); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	found := rxa.Match(query, true, true)
	keys, err := rxa.Sort(maps.Keys(found), desc, sort, vangogh_local_data.TitleProperty)

	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if err := encode(keys, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
