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
		if ramt, err := rxa.ReduxAssetsModTime(); err == nil {
			w.Header().Set(middleware.LastModifiedHeader, time.Unix(ramt, 0).Format(http.TimeFormat))
			ims := r.Header.Get(middleware.IfModifiedSinceHeader)
			if middleware.IsNotModified(ims, ramt) {
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
		if vr, err := vangogh_local_data.NewReader(pt); err == nil {
			if imt, err := vr.IndexCurrentModTime(); err == nil {
				w.Header().Set(middleware.LastModifiedHeader, time.Unix(imt, 0).Format(http.TimeFormat))
				if middleware.IsNotModified(r.Header.Get(middleware.IfModifiedSinceHeader), imt) {
					w.WriteHeader(http.StatusNotModified)
					return
				}
			}
		}
		next.ServeHTTP(w, r)
	})
}
