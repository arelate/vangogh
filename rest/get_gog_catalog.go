package rest

import (
	"net/http"
)

func GetGogCatalog(w http.ResponseWriter, r *http.Request) {

	// GET /gog/catalog

	getGogSection("/gog/catalog", w, r)
}
