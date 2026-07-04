package rest

import (
	"net/http"
	"path"
	"slices"
	"strconv"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetSearch(w http.ResponseWriter, r *http.Request) {

	// GET /search?(search_params)&from

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	from, to := 0, 0
	if q.Has(vangogh_integration.UrlFromParameter) {
		from64, err := strconv.ParseInt(q.Get(vangogh_integration.UrlFromParameter), 10, 32)
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

		sort := q.Get(vangogh_integration.UrlSortParameter)
		if sort == "" {
			sort = vangogh_integration.GogTitleProperty
		}
		desc := q.Get(vangogh_integration.UrlDescendingParameter) == "true"

		var found []string

		if isSortDescOnly(query) {
			found = slices.Collect(rdx.Keys(vangogh_integration.GogTitleProperty))
		} else {
			found = slices.Collect(rdx.Match(q))
		}

		var err error
		ids, err = rdx.Sort(found, desc, sort, vangogh_integration.GogTitleProperty, vangogh_integration.GogProductTypeProperty)
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
		http.Redirect(w, r, path.Join("/gog-product", ids[0]), http.StatusPermanentRedirect)
		return
	}

	permissions, err := sb.GetCookiePermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	searchPage := compton_pages.Search(query, ids, from, to, rdx, permissions...)
	if err = searchPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}

func isSortDescOnly(q map[string][]string) bool {
	switch len(q) {
	case 0:
		return false
	case 1:
		_, okSort := q[vangogh_integration.UrlSortParameter]
		_, okDesc := q[vangogh_integration.UrlDescendingParameter]
		return okSort || okDesc
	case 2:
		_, okSort := q[vangogh_integration.UrlSortParameter]
		_, okDesc := q[vangogh_integration.UrlDescendingParameter]
		return okSort && okDesc
	default:
		return false
	}

}
