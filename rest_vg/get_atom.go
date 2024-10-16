package rest_vg

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/middleware"
	"github.com/boggydigital/nod"
	"net/http"
	"os"
)

func GetAtom(w http.ResponseWriter, r *http.Request) {

	// GET /atom

	absAtomFeedPath, err := vangogh_local_data.AbsAtomFeedPath()
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if stat, err := os.Stat(absAtomFeedPath); err == nil {

		w.Header().Set(middleware.LastModifiedHeader, stat.ModTime().UTC().Format(http.TimeFormat))
		ims := r.Header.Get(middleware.IfModifiedSinceHeader)
		lm := stat.ModTime().UTC().Format(http.TimeFormat)
		if middleware.IsNotModified(ims, lm) {
			w.WriteHeader(http.StatusNotModified)
			return
		}

		http.ServeFile(w, r, absAtomFeedPath)
	} else {
		_ = nod.Error(fmt.Errorf("atom feed not found"))
		http.NotFound(w, r)
	}
}
