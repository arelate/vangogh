package http_api

import (
	"github.com/arelate/vangogh_urls"
	"net/http"
	"path"
)

func GetVideo(w http.ResponseWriter, r *http.Request) {
	videoId := path.Base(r.URL.Path)
	if localVideoPath := vangogh_urls.LocalVideoPath(videoId); localVideoPath != "" {
		http.ServeFile(w, r, localVideoPath)
	} else {
		w.WriteHeader(404)
	}
}
