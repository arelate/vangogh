package rest

import (
	"errors"
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func GetImage(w http.ResponseWriter, r *http.Request) {

	// GET /image?id

	q := r.URL.Query()
	imageId := q.Get("id")
	if imageId == "" {
		err := errors.New("empty image id")
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}
	if localImagePath, err := vangogh_integration.AbsLocalImagePath(imageId); err == nil && localImagePath != "" {
		w.Header().Set("Cache-Control", "max-age=31536000")
		http.ServeFile(w, r, localImagePath)
	} else {
		if err == nil {
			err = errors.New("no local image for id: " + imageId)
		}
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
