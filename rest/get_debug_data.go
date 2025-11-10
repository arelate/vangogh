package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetDebugData(w http.ResponseWriter, r *http.Request) {

	// GET /debug-data?id&product-type

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get(vangogh_integration.IdProperty)
	pts := r.URL.Query().Get(vangogh_integration.ProductTypeProperty)

	pt := vangogh_integration.ParseProductType(pts)

	if debugDataPage, err := compton_pages.DebugData(id, pt); debugDataPage != nil && err == nil {
		if err = debugDataPage.WriteResponse(w); err != nil {
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
