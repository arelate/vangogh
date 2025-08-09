package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetDebug(w http.ResponseWriter, r *http.Request) {

	// GET /debug?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	if debugPage, err := compton_pages.Debug(id, rdx); err == nil && debugPage != nil {
		if err = debugPage.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	} else {
		http.NotFound(w, r)
	}
}
