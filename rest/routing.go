package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/middleware"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
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
		// unauth data endpoints
		"GET /updates":        Log(http.HandlerFunc(GetUpdates)),
		"GET /product":        Log(http.HandlerFunc(GetProduct)),
		"GET /search":         Log(http.HandlerFunc(GetSearch)),
		"GET /properties":     Log(http.HandlerFunc(GetProperties)),
		"GET /external-links": Log(http.HandlerFunc(GetExternalLinks)),
		"GET /description":    Log(http.HandlerFunc(GetDescription)),
		"GET /downloads":      Log(http.HandlerFunc(GetDownloads)),
		"GET /changelog":      Log(http.HandlerFunc(GetChangelog)),
		"GET /screenshots":    Log(http.HandlerFunc(GetScreenshots)),
		"GET /videos":         Log(http.HandlerFunc(GetVideos)),
		"GET /steam-news":     Log(http.HandlerFunc(GetSteamNews)),
		"GET /steam-reviews":  Log(http.HandlerFunc(GetSteamReviews)),
		"GET /steam-deck":     Log(http.HandlerFunc(GetSteamDeck)),
		// unauth media endpoints
		"GET /image":  Log(http.HandlerFunc(GetImage)),
		"GET /items/": Log(http.HandlerFunc(GetItems)),
		// auth data endpoints
		"GET /wishlist/add":     Auth(Log(http.HandlerFunc(GetWishlistAdd)), AdminRole),
		"GET /wishlist/remove":  Auth(Log(http.HandlerFunc(GetWishlistRemove)), AdminRole),
		"GET /tags/edit":        Auth(Log(http.HandlerFunc(GetTagsEdit)), AdminRole),
		"GET /local-tags/edit":  Auth(Log(http.HandlerFunc(GetLocalTagsEdit)), AdminRole),
		"GET /tags/apply":       Auth(Log(http.HandlerFunc(GetTagsApply)), AdminRole),
		"GET /local-tags/apply": Auth(Log(http.HandlerFunc(GetLocalTagsApply)), AdminRole),
		// auth media endpoints
		"GET /files":       Auth(Log(http.HandlerFunc(GetFiles)), AdminRole, SharedRole),
		"GET /local-file/": Auth(Log(http.HandlerFunc(GetLocalFile)), AdminRole, SharedRole),
		// products redirects
		"GET /products": Redirect("/search", http.StatusPermanentRedirect),
		// API
		"GET /api/health":      Log(http.HandlerFunc(GetHealth)),
		"GET /api/health-auth": Auth(Log(http.HandlerFunc(GetHealth)), AdminRole, SharedRole),
		"GET /api/metadata":    Log(http.HandlerFunc(GetMetadata)),
		// start at the updates
		"GET /": Redirect("/updates", http.StatusPermanentRedirect),
	}

	for route, path := range searchRoutes() {
		patternHandlers["GET /products/"+route] = Redirect(path, http.StatusPermanentRedirect)
	}

	for p, h := range patternHandlers {
		http.HandleFunc(p, h.ServeHTTP)
	}
}

func searchRoutes() map[string]string {
	routes := make(map[string]string)

	searchPath := "/search"

	routes["filter"] = searchPath

	q := make(url.Values)
	q.Set(vangogh_integration.TypesProperty, vangogh_integration.AccountProducts.String())
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGOrderDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	routes["owned"] = searchPath + "?" + q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.WishlistedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGReleaseDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	routes["wishlist"] = searchPath + "?" + q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.TypesProperty, vangogh_integration.CatalogProducts.String())
	q.Set(vangogh_integration.OwnedProperty, vangogh_integration.FalseValue)
	q.Set(vangogh_integration.IsDiscountedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.DiscountPercentageProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	routes["sale"] = searchPath + "?" + q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.TypesProperty, vangogh_integration.CatalogProducts.String())
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGReleaseDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	routes["all"] = searchPath + "?" + q.Encode()

	return routes
}
