package rest

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetWishlistRemove(w http.ResponseWriter, r *http.Request) {

	// GET /wishlist/remove?id

	id := r.URL.Query().Get(vangogh_integration.IdProperty)

	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if pids, err := vangogh_integration.RemoveFromLocalWishlist([]string{id}, nil); err == nil {
		if err := gog_integration.RemoveFromWishlist(hc, pids...); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	http.Redirect(w, r, "/product?id="+id, http.StatusTemporaryRedirect)
}
