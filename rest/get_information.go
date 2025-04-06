package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetInformation(w http.ResponseWriter, r *http.Request) {

	// GET /information?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	if p := compton_pages.Information(id, rdx); p != nil {
		if err := p.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	}
}
