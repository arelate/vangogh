package rest

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
	"os"
	"time"
)

func GetAtom(w http.ResponseWriter, r *http.Request) {

	// GET /atom

	absAtomFeedPath := vangogh_local_data.AbsAtomFeedPath()
	if stat, err := os.Stat(absAtomFeedPath); err == nil {

		w.Header().Set(lastModifiedHeader, stat.ModTime().Format(time.RFC1123))
		ims := r.Header.Get(ifModifiedSinceHeader)
		if isNotModified(ims, stat.ModTime().Unix()) {
			w.WriteHeader(http.StatusNotModified)
			return
		}

		http.ServeFile(w, r, absAtomFeedPath)
	} else {
		_ = nod.Error(fmt.Errorf("atom feed not found"))
		http.NotFound(w, r)
	}
}
