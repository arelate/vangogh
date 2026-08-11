package rest

import (
	"net/http"
	"strings"

	"github.com/arelate/vangogh/rest/compton_data"
)

func GetGogSearch(w http.ResponseWriter, r *http.Request) {

	// GET /gog/search?(query)&from

	q := r.URL.Query()

	query := make(map[string][]string)

	shortQuery := false
	queryProperties := compton_data.SearchProperties
	for _, p := range queryProperties {
		if v := q.Get(p); v != "" {
			query[p] = strings.Split(v, ",")
		} else {
			if q.Has(p) {
				q.Del(p)
				shortQuery = true
			}
		}
	}

	//if we removed some properties with no values - redirect to the shortest URL
	if shortQuery {
		r.URL.RawQuery = q.Encode()
		http.Redirect(w, r, r.URL.String(), http.StatusPermanentRedirect)
		return
	}

	getGogSection("/gog/search", w, r)

}
