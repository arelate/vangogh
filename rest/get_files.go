package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetFiles(w http.ResponseWriter, r *http.Request) {

	// GET /files?manual-url

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	manualUrl := q.Get("manual-url")

	if manualUrl != "" {

		relLocalFilePath, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, manualUrl)
		if !ok {
			http.Error(w, nod.ErrorStr("no file for manual-url %s", manualUrl), http.StatusNotFound)
			return
		}

		http.Redirect(w, r, "/local-file/"+relLocalFilePath, http.StatusPermanentRedirect)
	} else {
		http.Error(w, nod.ErrorStr("missing manual-url"), http.StatusNotFound)
	}
}
