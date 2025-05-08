package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
	"slices"
	"strconv"
	"strings"
)

func GetSearch(w http.ResponseWriter, r *http.Request) {

	// GET /search?(search_params)&from

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	from, to := 0, 0
	if q.Has("from") {
		from64, err := strconv.ParseInt(q.Get("from"), 10, 32)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
		from = int(from64)
	}

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

	var ids []string

	if len(query) > 0 {

		sort := q.Get(vangogh_integration.SortProperty)
		if sort == "" {
			sort = vangogh_integration.TitleProperty
		}
		desc := q.Get(vangogh_integration.DescendingProperty) == "true"

		found := rdx.Match(q)

		var err error
		ids, err = rdx.Sort(slices.Collect(found), desc, sort, vangogh_integration.TitleProperty, vangogh_integration.ProductTypeProperty)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		if from > len(ids)-1 {
			from = 0
		}

		to = from + SearchResultsLimit
		if to > len(ids) {
			to = len(ids)
		} else if to+SearchResultsLimit > len(ids) {
			to = len(ids)
		}
	}

	if len(ids) == 1 {
		http.Redirect(w, r, "/product?id="+ids[0], http.StatusPermanentRedirect)
		return
	}

	searchPage := compton_pages.Search(query, ids, from, to, rdx)
	if err := searchPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
