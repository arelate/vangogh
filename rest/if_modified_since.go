package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/middleware"
	"net/http"
	"time"
)

func IfReduxModifiedSince(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		// redux assets mod time is used to:
		// 1) set Last-Modified header
		// 2) check if content was modified since client cache
		if ramt, err := rdx.ModTime(); err == nil {
			lm := time.Unix(ramt, 0).UTC().Format(http.TimeFormat)
			w.Header().Set(middleware.LastModifiedHeader, lm)
			ims := r.Header.Get(middleware.IfModifiedSinceHeader)
			if middleware.IsNotModified(ims, lm) {
				w.WriteHeader(http.StatusNotModified)
				return
			}
		}
		next.ServeHTTP(w, r)
	})
}

func IfDataModifiedSince(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		// data assets mod time is used to:
		// 1) set Last-Modified header
		// 2) check if content was modified since client cache
		//pt := vangogh_local_data.ProductTypeFromUrl(r.URL)
		pt := vangogh_local_data.ParseProductType(vangogh_local_data.ValueFromUrl(r.URL, "product-type"))
		if vr, err := vangogh_local_data.NewProductReader(pt); err == nil {
			if icmt, err := vr.IndexCurrentModTime(); err == nil {
				lm := time.Unix(icmt, 0).UTC().Format(http.TimeFormat)
				w.Header().Set(middleware.LastModifiedHeader, lm)
				if middleware.IsNotModified(r.Header.Get(middleware.IfModifiedSinceHeader), lm) {
					w.WriteHeader(http.StatusNotModified)
					return
				}
			}
		}
		next.ServeHTTP(w, r)
	})
}
