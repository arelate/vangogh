package rest

import (
	"net/http"
	"os"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetLogs(w http.ResponseWriter, r *http.Request) {

	// GET /logs?log

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	if strings.Contains(id, "/") ||
		strings.Contains(id, "\\") ||
		strings.Contains(id, "..") {
		http.Error(w, "Invalid log name", http.StatusBadRequest)
		return
	}

	if id != "" {

		absLogsDir := vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Logs)
		relFilename := filepath.Base(id)

		absFilepath := filepath.Join(absLogsDir, relFilename)
		if _, err := os.Stat(absFilepath); os.IsNotExist(err) {
			http.NotFound(w, r)
			return
		} else if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		w.Header().Set("Cache-Control", "max-age=31536000")
		w.Header().Set("Content-Disposition", "attachment; filename=\""+relFilename+"\"")
		http.ServeFile(w, r, absFilepath)
		return
	}

	logsPage := compton_pages.Logs(rdx)
	if err := logsPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
