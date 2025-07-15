package rest

import (
	_ "embed"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/middleware"
	"github.com/boggydigital/nod"
	"net/http"
)

var (
	Auth     = middleware.BasicHttpAuth
	Log      = nod.RequestLog
	Redirect = http.RedirectHandler
)

func HandleFuncs() {

	patternHandlers := map[string]http.Handler{
		// static resources
		"GET /manifest.json": Log(http.HandlerFunc(GetManifest)),
		"GET /icon.png":      Log(http.HandlerFunc(GetIcon)),
		"GET /atom":          Log(http.HandlerFunc(GetAtom)),
		// public data endpoints
		"GET /updates":       Log(http.HandlerFunc(GetUpdates)),
		"GET /search":        Log(http.HandlerFunc(GetSearch)),
		"GET /product":       Log(http.HandlerFunc(GetProduct)),
		"GET /info":          Log(http.HandlerFunc(GetInfo)),
		"GET /description":   Log(http.HandlerFunc(GetDescription)),
		"GET /reception":     Log(http.HandlerFunc(GetReception)),
		"GET /offerings":     Log(http.HandlerFunc(GetOfferings)),
		"GET /media":         Log(http.HandlerFunc(GetMedia)),
		"GET /news":          Log(http.HandlerFunc(GetNews)),
		"GET /changelog":     Log(http.HandlerFunc(GetChangelog)),
		"GET /compatibility": Log(http.HandlerFunc(GetCompatibility)),
		"GET /installers":    Log(http.HandlerFunc(GetInstallers)),
		"GET /wine-binaries": Log(http.HandlerFunc(GetWineBinaries)),
		// public media endpoints
		"GET /image":               Log(http.HandlerFunc(GetImage)),
		"GET /description-images/": Log(http.HandlerFunc(GetDescriptionImages)),
		// auth data endpoints
		"GET /wishlist/add":     Auth(Log(http.HandlerFunc(GetWishlistAdd)), AdminRole),
		"GET /wishlist/remove":  Auth(Log(http.HandlerFunc(GetWishlistRemove)), AdminRole),
		"GET /tags/edit":        Auth(Log(http.HandlerFunc(GetTagsEdit)), AdminRole),
		"GET /local-tags/edit":  Auth(Log(http.HandlerFunc(GetLocalTagsEdit)), AdminRole),
		"GET /tags/apply":       Auth(Log(http.HandlerFunc(GetTagsApply)), AdminRole),
		"GET /local-tags/apply": Auth(Log(http.HandlerFunc(GetLocalTagsApply)), AdminRole),
		// auth files endpoints
		"GET /files": Auth(Log(http.HandlerFunc(GetFiles)), AdminRole, SharedRole),
		// products redirects
		"GET /products": Redirect("/search", http.StatusPermanentRedirect),
		// API
		"GET /api/health":                 Log(http.HandlerFunc(GetHealth)),
		"GET /api/health-auth":            Auth(Log(http.HandlerFunc(GetHealth)), AdminRole, SharedRole),
		"GET /api/product-details":        Log(http.HandlerFunc(GetProductDetails)),
		"GET /api/wine-binary":            Log(http.HandlerFunc(GetWineBinary)),
		"GET /api/wine-binaries-versions": Log(http.HandlerFunc(GetWineBinariesVersions)),
		// debug endpoints
		"GET /debug":      Log(http.HandlerFunc(GetDebug)),
		"GET /debug-data": Auth(Log(http.HandlerFunc(GetDebugData)), AdminRole, SharedRole),
		// start at the updates
		"GET /": Redirect("/updates", http.StatusPermanentRedirect),
	}

	for route, path := range compton_data.SearchScopes() {
		patternHandlers["GET /products/"+route] = Redirect(path, http.StatusPermanentRedirect)
	}

	for p, h := range patternHandlers {
		http.HandleFunc(p, h.ServeHTTP)
	}
}
