package rest

import (
	"net/http"
)

func GetGogSearch(w http.ResponseWriter, r *http.Request) {

	// GET /gog/search?(query)&from

	getGogSection("/gog/search", w, r)
}
