package rest

import (
	_ "embed"
	"net/http"

	"github.com/arelate/vangogh/perm"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/author"
	"github.com/boggydigital/nod"
)

var (
	AuthSsn  = author.AuthenticateSession
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
		"GET /login":    Log(http.HandlerFunc(GetLogin)),
		"POST /auth":    Log(http.HandlerFunc(sb.AuthenticateUser)),
		"POST /success": AuthSsn(sb, Log(http.HandlerFunc(PostSuccess))),
		// soon to be authenticated endpoints
		"GET /updates":       AuthSsn(sb, Log(http.HandlerFunc(GetUpdates)), perm.ReadUpdates),
		"GET /search":        AuthSsn(sb, Log(http.HandlerFunc(GetSearch)), perm.ReadSearch),
		"GET /product":       AuthSsn(sb, Log(http.HandlerFunc(GetProduct)), perm.ReadProductData),
		"GET /info":          AuthSsn(sb, Log(http.HandlerFunc(GetInfo)), perm.ReadProductData),
		"GET /description":   AuthSsn(sb, Log(http.HandlerFunc(GetDescription)), perm.ReadProductData),
		"GET /reception":     AuthSsn(sb, Log(http.HandlerFunc(GetReception)), perm.ReadProductData),
		"GET /offerings":     AuthSsn(sb, Log(http.HandlerFunc(GetOfferings)), perm.ReadProductData),
		"GET /media":         AuthSsn(sb, Log(http.HandlerFunc(GetMedia)), perm.ReadProductData),
		"GET /news":          AuthSsn(sb, Log(http.HandlerFunc(GetNews)), perm.ReadProductData),
		"GET /changelog":     AuthSsn(sb, Log(http.HandlerFunc(GetChangelog)), perm.ReadProductData),
		"GET /compatibility": AuthSsn(sb, Log(http.HandlerFunc(GetCompatibility)), perm.ReadProductData),
		"GET /installers":    AuthSsn(sb, Log(http.HandlerFunc(GetInstallers)), perm.ReadFiles),
		"GET /wine-binaries": AuthSsn(sb, Log(http.HandlerFunc(GetWineBinaries)), perm.ReadFiles),
		// public media endpoints
		"GET /image":               AuthSsn(sb, Log(http.HandlerFunc(GetImage)), perm.ReadImages),
		"GET /description-images/": AuthSsn(sb, Log(http.HandlerFunc(GetDescriptionImages)), perm.ReadImages),
		// public files endpoints
		"GET /wine-binary-file": AuthSsn(sb, Log(http.HandlerFunc(GetWineBinaryFile)), perm.ReadFiles),
		// auth data endpoints
		"GET /wishlist/add":     AuthSsn(sb, Log(http.HandlerFunc(GetWishlistAdd)), perm.WriteWishlist),
		"GET /wishlist/remove":  AuthSsn(sb, Log(http.HandlerFunc(GetWishlistRemove)), perm.WriteWishlist),
		"GET /tags/edit":        AuthSsn(sb, Log(http.HandlerFunc(GetTagsEdit)), perm.WriteAccountTags),
		"GET /tags/apply":       AuthSsn(sb, Log(http.HandlerFunc(GetTagsApply)), perm.WriteAccountTags),
		"GET /local-tags/edit":  AuthSsn(sb, Log(http.HandlerFunc(GetLocalTagsEdit)), perm.WriteLocalTags),
		"GET /local-tags/apply": AuthSsn(sb, Log(http.HandlerFunc(GetLocalTagsApply)), perm.WriteLocalTags),
		// auth files endpoints
		"GET /files": AuthSsn(sb, Log(http.HandlerFunc(GetFiles)), perm.ReadFiles),
		// products redirects
		"GET /products": Redirect("/search", http.StatusPermanentRedirect),
		// API
		"GET /api/health":                 AuthSsn(sb, Log(http.HandlerFunc(GetHealth)), perm.ReadApi),
		"GET /api/product-details":        AuthSsn(sb, Log(http.HandlerFunc(GetProductDetails)), perm.ReadApi),
		"GET /api/wine-binaries-versions": AuthSsn(sb, Log(http.HandlerFunc(GetWineBinariesVersions)), perm.ReadApi),
		// debug endpoints
		"GET /debug":      AuthSsn(sb, Log(http.HandlerFunc(GetDebug)), perm.ReadDebug),
		"GET /debug-data": AuthSsn(sb, Log(http.HandlerFunc(GetDebugData)), perm.ReadDebug),
		"GET /logs":       AuthSsn(sb, Log(http.HandlerFunc(GetLogs)), perm.ReadLogs),
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
