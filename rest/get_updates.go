package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
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

	updatesPage := compton_pages.Updates(section, rdx, showAll)
	if err := updatesPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
