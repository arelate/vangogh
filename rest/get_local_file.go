package rest

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
	"os"
	"path/filepath"
)

func GetLocalFile(w http.ResponseWriter, r *http.Request) {

	// GET /local-file/{rel-local-path}

	localPath, err := filepath.Rel("/local-file/", r.URL.Path)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusMisdirectedRequest)
		return
	}

	if absLocalFilePath, err := vangogh_local_data.AbsDownloadDirFromRel(localPath); err == nil && absLocalFilePath != "" {
		if _, err := os.Stat(absLocalFilePath); err == nil {
			_, filename := filepath.Split(absLocalFilePath)
			w.Header().Set("Cache-Control", "max-age=31536000")
			w.Header().Set("Content-Disposition", "attachment; filename=\""+filename+"\"")
			http.ServeFile(w, r, absLocalFilePath)
		} else {
			http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
		}
	} else {
		if err == nil {
			err = fmt.Errorf("cannot resolve %s not found", localPath)
		}
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
