package rest

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"net/http"
	"path/filepath"
)

func GetItems(w http.ResponseWriter, r *http.Request) {

	// GET /items/{rel-local-path}

	localPath, err := filepath.Rel("/items/", r.URL.Path)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusMisdirectedRequest)
		return
	}

	if absLocalFilePath, err := vangogh_integration.AbsItemPath(localPath); err == nil && absLocalFilePath != "" {
		http.ServeFile(w, r, absLocalFilePath)
	} else {
		if err == nil {
			err = fmt.Errorf("file %s not found", absLocalFilePath)
		}
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
