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
		"GET /updates":     Log(http.HandlerFunc(GetUpdates)),
		"GET /product":     Log(http.HandlerFunc(GetProduct)),
		"GET /search":      Log(http.HandlerFunc(GetSearch)),
		"GET /information": Log(http.HandlerFunc(GetInformation)),
		"GET /offerings":   Log(http.HandlerFunc(GetOfferings)),
		"GET /links":       Log(http.HandlerFunc(GetLinks)),
		"GET /description": Log(http.HandlerFunc(GetDescription)),
		"GET /installers":  Log(http.HandlerFunc(GetInstallers)),
		"GET /changelog":   Log(http.HandlerFunc(GetChangelog)),
		"GET /screenshots": Log(http.HandlerFunc(GetScreenshots)),
		"GET /videos":      Log(http.HandlerFunc(GetVideos)),
		"GET /news":        Log(http.HandlerFunc(GetNews)),
		"GET /reception":   Log(http.HandlerFunc(GetReception)),
		"GET /steam-deck":  Log(http.HandlerFunc(GetSteamDeck)),
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
		"GET /api/health":              Log(http.HandlerFunc(GetHealth)),
		"GET /api/health-auth":         Auth(Log(http.HandlerFunc(GetHealth)), AdminRole, SharedRole),
		"GET /api/product-details":     Log(http.HandlerFunc(GetProductDetails)),
		"GET /api/github-releases":     Log(http.HandlerFunc(GetGitHubReleases)),
		"GET /api/github-latest-asset": Log(http.HandlerFunc(GetGitHubLatestAsset)),
		// debug endpoints
		"GET /debug":                Log(http.HandlerFunc(GetDebug)),
		"GET /debug-data":           Auth(Log(http.HandlerFunc(GetDebugData)), AdminRole, SharedRole),
		"GET /debug-get-data-redux": Log(http.HandlerFunc(GetDebugGetDataRedux)),
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
