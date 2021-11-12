package v1

import (
	"github.com/arelate/vangogh_urls"
	"net/http"
	"path"
)

func GetImage(w http.ResponseWriter, r *http.Request) {

	// GET /v1/image?id

	imageId := path.Base(r.URL.Path)
	if localImagePath := vangogh_urls.LocalImagePath(imageId); localImagePath != "" {
		http.ServeFile(w, r, localImagePath)
	} else {
		w.WriteHeader(404)
	}
}
