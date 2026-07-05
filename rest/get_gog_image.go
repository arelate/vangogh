package rest

import (
	"errors"
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/nod"
)

func GetGogImage(w http.ResponseWriter, r *http.Request) {

	// GET /gog-image/{imageId}

	imageId := r.PathValue("imageId")

	if imageId == "" {
		err := errors.New("empty image id")
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	if localImagePath, err := vangogh_integration.AbsLocalImagePath(imageId); err == nil && localImagePath != "" {
		camino.ServeFile(w, r, localImagePath, camino.NoContentDisposition, camino.NoBinaryContentType)
	} else {
		if err == nil {
			err = errors.New("no local image for id: " + imageId)
		}
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
