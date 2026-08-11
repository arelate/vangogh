package rest

import (
	"net/http"
)

func GetGogWishlist(w http.ResponseWriter, r *http.Request) {

	// GET /gog/wishlist

	getGogSection("/gog/wishlist", w, r)
}
