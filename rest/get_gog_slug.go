package rest

import (
	"net/http"
	"net/url"
	"path"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogSlug(w http.ResponseWriter, r *http.Request) {

	// GET /gog-slug/{slug}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	slug := r.PathValue(vangogh_integration.UrlSlugParameter)

	q := url.Values{}
	q.Set(vangogh_integration.GogSlugProperty, slug)

	for id := range rdx.Match(q, redux.FullMatch) {
		http.Redirect(w, r, path.Join("/gog-product", id), http.StatusPermanentRedirect)
		return
	}

	http.Error(w, nod.ErrorStr("unknown slug", slug), http.StatusInternalServerError)
}
