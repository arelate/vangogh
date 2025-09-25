package rest

import (
	"fmt"
	"net/http"
	"os"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/middleware"
	"github.com/boggydigital/nod"
)

func GetAtom(w http.ResponseWriter, r *http.Request) {

	// GET /atom

	absAtomFeedPath, err := vangogh_integration.AbsAtomFeedPath()
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if stat, err := os.Stat(absAtomFeedPath); err == nil {

		w.Header().Set(middleware.LastModifiedHeader, stat.ModTime().UTC().Format(http.TimeFormat))
		ifModifiedSince := r.Header.Get(middleware.IfModifiedSinceHeader)
		lastModified := stat.ModTime().UTC().Format(http.TimeFormat)

		if ims, err := time.Parse(http.TimeFormat, ifModifiedSince); err == nil {
			if lm, err := time.Parse(http.TimeFormat, lastModified); err == nil {
				if lm.Unix() <= ims.Unix() {
					w.WriteHeader(http.StatusNotModified)
					return
				}
			}
		}
		http.ServeFile(w, r, absAtomFeedPath)
	} else {
		nod.LogError(fmt.Errorf("atom feed not found"))
		http.NotFound(w, r)
	}
}
