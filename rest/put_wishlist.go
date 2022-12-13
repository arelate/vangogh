package rest

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/http"
)

func PutWishlist(
	httpClient *http.Client,
	ids map[string]bool,
	w http.ResponseWriter) {

	// PUT /wishlist?id

	if len(ids) > 0 {
		if pids, err := vangogh_local_data.AddToLocalWishlist(maps.Keys(ids), nil); err == nil {
			if err := gog_integration.AddToWishlist(httpClient, pids...); err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
		} else {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	w.WriteHeader(http.StatusOK)
}
