package rest

import (
	"net/http"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
)

func GetWishlistAdd(w http.ResponseWriter, r *http.Request) {

	// GET /wishlist/add?id

	id := r.URL.Query().Get(vangogh_integration.IdProperty)

	acp := vangogh_integration.AbsCookiePath()

	jar, err := coost.Read(gog_integration.DefaultUrl(), acp)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	hc := http.DefaultClient
	hc.Jar = jar

	if pids, err := vangogh_integration.AddToLocalWishlist([]string{id}, nil); err == nil {
		if err := gog_integration.AddToWishlist(hc, pids...); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	http.Redirect(w, r, "/product?id="+id, http.StatusTemporaryRedirect)
}
