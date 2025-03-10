package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetDebugGetDataErrors(w http.ResponseWriter, r *http.Request) {

	// GET /debug-get-data-errors?id

	id := r.URL.Query().Get(vangogh_integration.IdProperty)

	if debugGetDataErrorsPage, err := compton_pages.DebugGetDataErrors(id); debugGetDataErrorsPage != nil && err == nil {
		if err = debugGetDataErrorsPage.WriteResponse(w); err != nil {
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
