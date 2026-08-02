package rest

import (
	"net/http"
	"path"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
)

func GetGogWishlistAdd(w http.ResponseWriter, r *http.Request) {

	// GET /gog-wishlist/add?id

	id := r.URL.Query().Get(vangogh_integration.UrlIdParameter)
	if !isAllowed(id, digits) {
		http.Error(w, errCharactersNotAllowed, http.StatusBadRequest)
		return
	}

	jar, err := coost.Read(gog_integration.HostUrl(), vangogh_integration.AbsCookiesPath())
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	hc := http.DefaultClient
	hc.Jar = jar

	var pids []string
	if pids, err = vangogh_integration.AddToLocalWishlist([]string{id}, nil); err == nil {
		if err = gog_integration.AddToWishlist(hc, pids...); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	http.Redirect(w, r, path.Join("/gog-product", id), http.StatusTemporaryRedirect)
}
