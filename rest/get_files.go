package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"net/http"
	"os"
	"path/filepath"
)

func GetFiles(w http.ResponseWriter, r *http.Request) {

	// GET /files?id&download-type&manual-url

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	id := q.Get("id")
	manualUrl := q.Get("manual-url")
	downloadType := vangogh_integration.ParseDownloadType(q.Get("download-type"))

	if id == "" {
		http.Error(w, nod.ErrorStr("missing id"), http.StatusBadRequest)
		return
	}

	if manualUrl == "" {
		http.Error(w, nod.ErrorStr("missing manual-url"), http.StatusBadRequest)
		return
	}

	if downloadType == vangogh_integration.AnyDownloadType {
		http.Error(w, nod.ErrorStr("undefined download-type"), http.StatusBadRequest)
		return
	}

	slug, ok := rdx.GetLastVal(vangogh_integration.SlugProperty, id)
	if !ok || slug == "" {
		http.Error(w, nod.ErrorStr("no slug for id %s", id), http.StatusNotFound)
		return
	}

	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, downloadType, downloadsLayout)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	filename, ok := rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, manualUrl)
	if !ok || filename == "" {
		http.Error(w, nod.ErrorStr("no filename for manual-url %s", manualUrl), http.StatusNotFound)
		return
	}

	absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

	if _, err := os.Stat(absDownloadPath); err == nil {
		w.Header().Set("Cache-Control", "max-age=31536000")
		w.Header().Set("Content-Disposition", "attachment; filename=\""+filename+"\"")
		http.ServeFile(w, r, absDownloadPath)
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
