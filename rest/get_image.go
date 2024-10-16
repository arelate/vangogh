package rest

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetImage(w http.ResponseWriter, r *http.Request) {

	// GET /image?id

	q := r.URL.Query()
	imageId := q.Get("id")
	if imageId == "" {
		err := fmt.Errorf("empty image id")
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}
	if localImagePath, err := vangogh_local_data.AbsLocalImagePath(imageId); err == nil && localImagePath != "" {
		w.Header().Set("Cache-Control", "max-age=31536000")
		http.ServeFile(w, r, localImagePath)
	} else {
		if err == nil {
			err = fmt.Errorf("no local image for id %s", imageId)
		}
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
