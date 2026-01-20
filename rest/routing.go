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
	AuthSt   = author.AuthSessionToken
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
		"GET /login":  Log(http.HandlerFunc(GetLogin)),
		"GET /logout": Log(http.HandlerFunc(sb.DeauthSession)),
		"POST /auth":  Log(http.HandlerFunc(sb.AuthBrowserUsernamePassword)),
		// authenticated endpoints
		"GET /updates":       AuthSt(sb, Log(http.HandlerFunc(GetUpdates)), perm.ReadUpdates),
		"GET /search":        AuthSt(sb, Log(http.HandlerFunc(GetSearch)), perm.ReadSearch),
		"GET /product":       AuthSt(sb, Log(http.HandlerFunc(GetProduct)), perm.ReadProductData),
		"GET /info":          AuthSt(sb, Log(http.HandlerFunc(GetInfo)), perm.ReadProductData),
		"GET /description":   AuthSt(sb, Log(http.HandlerFunc(GetDescription)), perm.ReadProductData),
		"GET /reception":     AuthSt(sb, Log(http.HandlerFunc(GetReception)), perm.ReadProductData),
		"GET /offerings":     AuthSt(sb, Log(http.HandlerFunc(GetOfferings)), perm.ReadProductData),
		"GET /media":         AuthSt(sb, Log(http.HandlerFunc(GetMedia)), perm.ReadProductData),
		"GET /news":          AuthSt(sb, Log(http.HandlerFunc(GetNews)), perm.ReadProductData),
		"GET /changelog":     AuthSt(sb, Log(http.HandlerFunc(GetChangelog)), perm.ReadProductData),
		"GET /compatibility": AuthSt(sb, Log(http.HandlerFunc(GetCompatibility)), perm.ReadProductData),
		"GET /installers":    AuthSt(sb, Log(http.HandlerFunc(GetInstallers)), perm.ReadFiles),
		"GET /binaries":      AuthSt(sb, Log(http.HandlerFunc(GetBinaries)), perm.ReadFiles),
		// public media endpoints
		"GET /image":               AuthSt(sb, Log(http.HandlerFunc(GetImage)), perm.ReadImages),
		"GET /description-images/": AuthSt(sb, Log(http.HandlerFunc(GetDescriptionImages)), perm.ReadImages),
		// binaries endpoints
		"GET /wine-binary-file":     AuthSt(sb, Log(http.HandlerFunc(GetWineBinaryFile)), perm.ReadFiles),
		"GET /steamcmd-binary-file": AuthSt(sb, Log(http.HandlerFunc(GetSteamCmdBinaryFile)), perm.ReadFiles),
		// auth data endpoints
		"GET /wishlist/add":     AuthSt(sb, Log(http.HandlerFunc(GetWishlistAdd)), perm.WriteWishlist),
		"GET /wishlist/remove":  AuthSt(sb, Log(http.HandlerFunc(GetWishlistRemove)), perm.WriteWishlist),
		"GET /tags/edit":        AuthSt(sb, Log(http.HandlerFunc(GetTagsEdit)), perm.WriteTagId),
		"GET /tags/apply":       AuthSt(sb, Log(http.HandlerFunc(GetTagsApply)), perm.WriteTagId),
		"GET /local-tags/edit":  AuthSt(sb, Log(http.HandlerFunc(GetLocalTagsEdit)), perm.WriteLocalTags),
		"GET /local-tags/apply": AuthSt(sb, Log(http.HandlerFunc(GetLocalTagsApply)), perm.WriteLocalTags),
		// auth files endpoints
		"GET /files": AuthSt(sb, Log(http.HandlerFunc(GetFiles)), perm.ReadFiles),
		// products redirects
		"GET /products": Redirect("/search", http.StatusPermanentRedirect),
		// API
		"POST /api/auth-user":             Log(http.HandlerFunc(sb.AuthApiUsernamePassword)),
		"POST /api/auth-session":          Log(http.HandlerFunc(sb.AuthApiSession)),
		"GET /api/product-details":        AuthSt(sb, Log(http.HandlerFunc(GetProductDetails)), perm.ReadApi),
		"GET /api/wine-binaries-versions": AuthSt(sb, Log(http.HandlerFunc(GetWineBinariesVersions)), perm.ReadApi),
		// debug endpoints
		"GET /debug":           AuthSt(sb, Log(http.HandlerFunc(GetDebug)), perm.ReadDebug),
		"GET /debug-data":      AuthSt(sb, Log(http.HandlerFunc(GetDebugData)), perm.ReadDebug),
		"GET /logs":            AuthSt(sb, Log(http.HandlerFunc(GetLogs)), perm.ReadLogs),
		"GET /downloads-queue": AuthSt(sb, Log(http.HandlerFunc(GetDownloadsQueue)), perm.ReadDebug),
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
