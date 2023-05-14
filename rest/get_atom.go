package rest

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

	absAtomFeedPath := vangogh_local_data.AbsAtomFeedPath()
	if stat, err := os.Stat(absAtomFeedPath); err == nil {

		w.Header().Set(middleware.LastModifiedHeader, stat.ModTime().Format(http.TimeFormat))
		ims := r.Header.Get(middleware.IfModifiedSinceHeader)
		if middleware.IsNotModified(ims, stat.ModTime().Unix()) {
			w.WriteHeader(http.StatusNotModified)
			return
		}

		http.ServeFile(w, r, absAtomFeedPath)
	} else {
		_ = nod.Error(fmt.Errorf("atom feed not found"))
		http.NotFound(w, r)
	}
}
