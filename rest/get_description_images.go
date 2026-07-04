package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/nod"
)

func GetDescriptionImage(w http.ResponseWriter, r *http.Request) {

	// GET /description-image/{relPath...}

	relPath := r.PathValue("relPath")

	if absLocalFilePath, err := vangogh_integration.AbsDescriptionImagePath(relPath); err == nil {
		camino.ServeFile(absLocalFilePath, w, r)
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
