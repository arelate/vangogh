package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetCompatibility(w http.ResponseWriter, r *http.Request) {

	// GET /compatibility?id

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

	p := compton_pages.Compatibility(id, rdx, permissions...)
	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
