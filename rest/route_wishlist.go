package rest

import (
	"github.com/arelate/gog_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
	"net/http"
)

func RouteWishlist(w http.ResponseWriter, r *http.Request) {

	ids, err := vangogh_local_data.IdSetFromUrl(r.URL)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	hc, err := coost.NewHttpClientFromFile(vangogh_local_data.AbsCookiePath(), gog_integration.GogHost)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	switch r.Method {
	case http.MethodPut:
		PutWishlist(hc, ids, w)
	case http.MethodDelete:
		DeleteWishlist(hc, ids, w)
	default:
		http.Error(w, "unexpected wishlist method", http.StatusMethodNotAllowed)
	}
}
