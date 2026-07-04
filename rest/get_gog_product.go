package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetGogProduct(w http.ResponseWriter, r *http.Request) {

	// GET /gog-product/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)

	sessionPermissions, err := sb.GetCookiePermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	if productPage := compton_pages.GogProduct(id, rdx, sessionPermissions...); productPage != nil {
		if err = productPage.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	} else {
		http.NotFound(w, r)
	}
}
