package rest_vg

import (
	"github.com/boggydigital/nod"
	"net/http"
)

var (
	Log = nod.RequestLog
)

func HandleFuncs() {
	patternHandlers := map[string]http.Handler{
		"GET /atom":        Log(http.HandlerFunc(GetAtom)),
		"GET /data":        IfDataModifiedSince(Log(http.HandlerFunc(GetData))),
		"GET /digest":      IfReduxModifiedSince(Log(http.HandlerFunc(GetDigest))),
		"GET /downloads":   IfDataModifiedSince(Log(http.HandlerFunc(GetDownloads))),
		"GET /has_data":    IfReduxModifiedSince(Log(http.HandlerFunc(GetHasData))),
		"GET /has_redux":   IfReduxModifiedSince(Log(http.HandlerFunc(GetHasRedux))),
		"GET /health":      Log(http.HandlerFunc(GetHealth)),
		"PATCH /local_tag": Log(http.HandlerFunc(PatchLocalTag)),
		"GET /redux":       IfReduxModifiedSince(Log(http.HandlerFunc(GetRedux))),
		"GET /search":      IfReduxModifiedSince(Log(http.HandlerFunc(Search))),
		"PATCH /tag":       Log(http.HandlerFunc(PatchTag)),
		"GET /titles":      IfReduxModifiedSince(Log(http.HandlerFunc(GetTitles))),
		"/wishlist":        Log(http.HandlerFunc(RouteWishlist)),
	}

	for p, h := range patternHandlers {
		http.HandleFunc(p, h.ServeHTTP)
	}
}
