package rest

import (
	"net/http"
)

func GetGogSale(w http.ResponseWriter, r *http.Request) {

	// GET /gog/sale

	getGogSection("/gog/sale", w, r)
}
