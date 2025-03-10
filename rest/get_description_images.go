package rest

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"net/http"
	"path/filepath"
)

func GetDescriptionImages(w http.ResponseWriter, r *http.Request) {

	// GET /description-images/{rel-local-path}

	localPath, err := filepath.Rel("/description-images/", r.URL.Path)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusMisdirectedRequest)
		return
	}

	if absLocalFilePath, err := vangogh_integration.AbsDescriptionImagePath(localPath); err == nil && absLocalFilePath != "" {
		http.ServeFile(w, r, absLocalFilePath)
	} else {
		if err == nil {
			err = fmt.Errorf("file %s not found", absLocalFilePath)
		}
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
