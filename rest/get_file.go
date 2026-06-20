package rest

import (
	"net/http"
	"os"
	"path/filepath"
	"strconv"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func GetFile(w http.ResponseWriter, r *http.Request) {

	// GET /file?id&download-type&manual-url

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	id := q.Get(vangogh_integration.UrlIdParameter)
	manualUrl := q.Get(vangogh_integration.UrlManualUrlParameter)
	downloadType := vangogh_integration.ParseDownloadType(q.Get(vangogh_integration.UrlDownloadTypeParameter))

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

	slug, ok := rdx.GetLastVal(vangogh_integration.GogSlugProperty, id)
	if !ok || slug == "" {
		http.Error(w, nod.ErrorStr("no slug for id %s", id), http.StatusNotFound)
		return
	}

	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, downloadType, downloadsLayout)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	filename, ok := rdx.GetLastVal(vangogh_integration.GogManualUrlFilenameProperty, manualUrl)
	if !ok || filename == "" {
		http.Error(w, nod.ErrorStr("no filename for manual-url %s", manualUrl), http.StatusNotFound)
		return
	}

	absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

	var fi os.FileInfo
	if fi, err = os.Stat(absDownloadPath); err == nil {
		w.Header().Set("Cache-Control", "max-age=31536000")
		w.Header().Set("Content-Disposition", "attachment; filename=\""+filename+"\"")
		w.Header().Set("Content-Length", strconv.FormatInt(fi.Size(), 10))
		http.ServeFile(w, r, absDownloadPath)
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusNotFound)
	}
}
