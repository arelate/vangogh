package rest

import (
	"github.com/boggydigital/middleware"
	"github.com/boggydigital/nod"
	"net/http"
)

var (
	GetOnly   = middleware.GetMethodOnly
	PatchOnly = middleware.PatchMethodOnly
	Log       = nod.RequestLog
)

func HandleFuncs() {
	patternHandlers := map[string]http.Handler{
		"/atom":      GetOnly(Log(http.HandlerFunc(GetAtom))),
		"/data":      IfDataModifiedSince(GetOnly(Log(http.HandlerFunc(GetData)))),
		"/digest":    IfReduxModifiedSince(GetOnly(Log(http.HandlerFunc(GetDigest)))),
		"/downloads": IfDataModifiedSince(GetOnly(Log(http.HandlerFunc(GetDownloads)))),
		"/has_data":  IfReduxModifiedSince(GetOnly(Log(http.HandlerFunc(GetHasData)))),
		"/has_redux": IfReduxModifiedSince(GetOnly(Log(http.HandlerFunc(GetHasRedux)))),
		"/local_tag": PatchOnly(Log(http.HandlerFunc(PatchLocalTag))),
		"/redux":     IfReduxModifiedSince(GetOnly(Log(http.HandlerFunc(GetRedux)))),
		"/search":    IfReduxModifiedSince(GetOnly(Log(http.HandlerFunc(Search)))),
		"/tag":       PatchOnly(Log(http.HandlerFunc(PatchTag))),
		"/wishlist":  Log(http.HandlerFunc(RouteWishlist)),
	}

	for p, h := range patternHandlers {
		http.HandleFunc(p, h.ServeHTTP)
	}
}
