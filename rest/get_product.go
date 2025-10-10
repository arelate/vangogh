package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetProduct(w http.ResponseWriter, r *http.Request) {

	// GET /product?slug&steam-app-id -> /product?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	redirectProperties := []string{vangogh_integration.SlugProperty, vangogh_integration.SteamAppIdProperty}

	for _, rp := range redirectProperties {
		if q.Has(rp) {
			if ids := rdx.Match(q, redux.FullMatch); ids != nil {
				for id := range ids {
					http.Redirect(w, r, "/product?id="+id, http.StatusPermanentRedirect)
					return
				}
			} else {
				http.Error(w, nod.ErrorStr("unknown %s", rp), http.StatusInternalServerError)
				return
			}
		}
	}

	id := r.URL.Query().Get(vangogh_integration.IdProperty)

	sessionPermissions, err := sb.GetPermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	if productPage := compton_pages.Product(id, rdx, sessionPermissions...); productPage != nil {
		if err := productPage.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	} else {
		http.NotFound(w, r)
	}
}
