package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetDebugData(w http.ResponseWriter, r *http.Request) {

	// GET /debug-data/{productType}/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue("id")
	pts := r.PathValue("productType")

	pt := vangogh_integration.ParseProductType(pts)

	if pt == vangogh_integration.UnknownProductType {
		http.Error(w, "unknown product type "+pts, http.StatusBadRequest)
		return
	}

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
