package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetInfo(w http.ResponseWriter, r *http.Request) {

	// GET /info?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	permissions, err := sb.GetPermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if p := compton_pages.Info(id, rdx, permissions...); p != nil {
		if err := p.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	}
}
