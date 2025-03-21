package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"net/http"
)

func GetProduct(w http.ResponseWriter, r *http.Request) {

	// GET /product?slug -> /product?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if r.URL.Query().Has(vangogh_integration.SlugProperty) {
		if ids := rdx.Match(r.URL.Query(), redux.FullMatch); ids != nil {
			for id := range ids {
				http.Redirect(w, r, "/product?id="+id, http.StatusPermanentRedirect)
				return
			}
		} else {
			http.Error(w, nod.ErrorStr("unknown slug"), http.StatusInternalServerError)
			return
		}
	}

	id := r.URL.Query().Get(vangogh_integration.IdProperty)

	if productPage := compton_pages.Product(id, rdx); productPage != nil {
		if err := productPage.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	} else {
		http.NotFound(w, r)
	}
}
