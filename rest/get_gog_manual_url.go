package rest

import (
	"net/http"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/nod"
)

func GetGogManualUrl(w http.ResponseWriter, r *http.Request) {

	// GET /gog-manual-url/{manualUrl...}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)
	downloadType := vangogh_integration.ParseDownloadType(r.PathValue("dt"))
	manualUrl := r.PathValue("mu")
	if !strings.HasPrefix(manualUrl, "/") {
		manualUrl = "/" + manualUrl
	}

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

	camino.ServeFile(w, r, absDownloadPath)
}
