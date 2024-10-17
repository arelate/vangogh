package rest

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/vangogh/paths"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetWishlistAdd(w http.ResponseWriter, r *http.Request) {

	// GET /wishlist/add?id

	id := r.URL.Query().Get(vangogh_local_data.IdProperty)

	acp, err := vangogh_local_data.AbsCookiePath()
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if pids, err := vangogh_local_data.AddToLocalWishlist([]string{id}, nil); err == nil {
		if err := gog_integration.AddToWishlist(hc, pids...); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	http.Redirect(w, r, paths.ProductId(id), http.StatusTemporaryRedirect)
}
