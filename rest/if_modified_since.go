package rest

import (
	"github.com/arelate/vangogh_local_data"
	"net/http"
	"time"
)

const (
	lastModifiedHeader    = "Last-Modified"
	ifModifiedSinceHeader = "If-Modified-Since"
)

func isNotModified(ifModifiedSince string, since int64) bool {
	utcSince := time.Unix(since, 0).UTC()
	if ims, err := time.Parse(time.RFC1123, ifModifiedSince); err == nil {
		return utcSince.Unix() <= ims.UTC().Unix()
	}
	return false
}

func IfReduxModifiedSince(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		// redux assets mod time is used to:
		// 1) set Last-Modified header
		// 2) check if content was modified since client cache
		if ramt, err := rxa.ReduxAssetsModTime(); err == nil {
			w.Header().Set(lastModifiedHeader, time.Unix(ramt, 0).Format(time.RFC1123))
			ims := r.Header.Get(ifModifiedSinceHeader)
			if isNotModified(ims, ramt) {
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
				w.Header().Set(lastModifiedHeader, time.Unix(imt, 0).Format(time.RFC1123))
				if isNotModified(r.Header.Get(ifModifiedSinceHeader), imt) {
					w.WriteHeader(http.StatusNotModified)
					return
				}
			}
		}
		next.ServeHTTP(w, r)
	})
}
