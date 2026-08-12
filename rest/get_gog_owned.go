package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetGogOwned(w http.ResponseWriter, r *http.Request) {

	// GET /gog/owned

	getGogSection("/gog/owned", w, r)
}

func getGogSection(sectionUrl string, w http.ResponseWriter, r *http.Request) {

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	permissions, err := sb.GetCookiePermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	sortBy := r.PathValue("sortBy")

	gogSectionPage := compton_pages.GogSearch(sectionUrl, sortBy, r.URL.Query(), rdx, permissions...)
	if err = gogSectionPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
