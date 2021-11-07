package http_api

import (
	"github.com/arelate/vangogh_urls"
	"net/http"
	"path"
)

func GetImage(w http.ResponseWriter, r *http.Request) {
	imageId := path.Base(r.URL.Path)
	if localImagePath := vangogh_urls.LocalImagePath(imageId); localImagePath != "" {
		http.ServeFile(w, r, localImagePath)
	} else {
		w.WriteHeader(404)
	}
}
