package rest

import (
	"errors"
	"net/http"
	"os"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

const (
	LastModifiedHeader    = "Last-Modified"
	IfModifiedSinceHeader = "If-Modified-Since"
)

func GetAtom(w http.ResponseWriter, r *http.Request) {

	// GET /atom

	absAtomFeedPath := vangogh_integration.AbsAtomFeedPath()
	if stat, err := os.Stat(absAtomFeedPath); err == nil {

		w.Header().Set(LastModifiedHeader, stat.ModTime().UTC().Format(http.TimeFormat))
		ifModifiedSince := r.Header.Get(IfModifiedSinceHeader)
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
		nod.LogError(errors.New("atom feed not found"))
		http.NotFound(w, r)
	}
}
