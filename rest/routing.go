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
	AuthCookie = author.AuthSessionCookie
	AuthBearer = author.AuthSessionBearer
	Log        = nod.RequestLog
	Redirect   = http.RedirectHandler
)

func HandleFuncs() {

	patternHandlers := map[string]http.Handler{
		// static resources
		"GET /manifest.json": Log(http.HandlerFunc(GetManifest)),
		"GET /icon.png":      Log(http.HandlerFunc(GetIcon)),
		"GET /atom":          Log(http.HandlerFunc(GetAtom)),
		// public data endpoints
		"GET /login":  Log(http.HandlerFunc(GetLogin)),
		"GET /logout": Log(http.HandlerFunc(sb.DeauthCookieSession)),
		"POST /auth":  Log(http.HandlerFunc(sb.AuthBrowserUsernamePassword)),
		// authenticated endpoints
		"GET /updates":       AuthCookie(sb, Log(http.HandlerFunc(GetUpdates)), perm.ReadUpdates),
		"GET /search":        AuthCookie(sb, Log(http.HandlerFunc(GetSearch)), perm.ReadSearch),
		"GET /product":       AuthCookie(sb, Log(http.HandlerFunc(GetProduct)), perm.ReadProductData),
		"GET /info":          AuthCookie(sb, Log(http.HandlerFunc(GetInfo)), perm.ReadProductData),
		"GET /description":   AuthCookie(sb, Log(http.HandlerFunc(GetDescription)), perm.ReadProductData),
		"GET /reception":     AuthCookie(sb, Log(http.HandlerFunc(GetReception)), perm.ReadProductData),
		"GET /offerings":     AuthCookie(sb, Log(http.HandlerFunc(GetOfferings)), perm.ReadProductData),
		"GET /media":         AuthCookie(sb, Log(http.HandlerFunc(GetMedia)), perm.ReadProductData),
		"GET /news":          AuthCookie(sb, Log(http.HandlerFunc(GetNews)), perm.ReadProductData),
		"GET /changelog":     AuthCookie(sb, Log(http.HandlerFunc(GetChangelog)), perm.ReadProductData),
		"GET /compatibility": AuthCookie(sb, Log(http.HandlerFunc(GetCompatibility)), perm.ReadProductData),
		"GET /installers":    AuthCookie(sb, Log(http.HandlerFunc(GetInstallers)), perm.ReadFiles),
		"GET /binaries":      AuthCookie(sb, Log(http.HandlerFunc(GetBinaries)), perm.ReadFiles),
		// public media endpoints
		"GET /image":               AuthCookie(sb, Log(http.HandlerFunc(GetImage)), perm.ReadImages),
		"GET /description-images/": AuthCookie(sb, Log(http.HandlerFunc(GetDescriptionImages)), perm.ReadImages),
		// binaries endpoints
		"GET /wine-binary-file":     AuthCookie(sb, Log(http.HandlerFunc(GetWineBinaryFile)), perm.ReadFiles),
		"GET /steamcmd-binary-file": AuthCookie(sb, Log(http.HandlerFunc(GetSteamCmdBinaryFile)), perm.ReadFiles),
		// auth data endpoints
		"GET /wishlist/add":     AuthCookie(sb, Log(http.HandlerFunc(GetWishlistAdd)), perm.WriteWishlist),
		"GET /wishlist/remove":  AuthCookie(sb, Log(http.HandlerFunc(GetWishlistRemove)), perm.WriteWishlist),
		"GET /tags/edit":        AuthCookie(sb, Log(http.HandlerFunc(GetTagsEdit)), perm.WriteTagId),
		"GET /tags/apply":       AuthCookie(sb, Log(http.HandlerFunc(GetTagsApply)), perm.WriteTagId),
		"GET /local-tags/edit":  AuthCookie(sb, Log(http.HandlerFunc(GetLocalTagsEdit)), perm.WriteLocalTags),
		"GET /local-tags/apply": AuthCookie(sb, Log(http.HandlerFunc(GetLocalTagsApply)), perm.WriteLocalTags),
		// auth files endpoints
		"GET /file": AuthCookie(sb, Log(http.HandlerFunc(GetFile)), perm.ReadFiles),
		// products redirects
		"GET /products": Redirect("/search", http.StatusPermanentRedirect),
		// API
		"POST /api/auth-user":             Log(http.HandlerFunc(sb.AuthApiUsernamePassword)),
		"POST /api/auth-session":          Log(http.HandlerFunc(sb.AuthApiSession)),
		"GET /api/available-products":     AuthBearer(sb, Log(http.HandlerFunc(GetAvailableProducts)), perm.ReadApi, perm.ReadProductData),
		"GET /api/product-details":        AuthBearer(sb, Log(http.HandlerFunc(GetProductDetails)), perm.ReadApi, perm.ReadProductData),
		"GET /api/manual-url-checksums":   AuthBearer(sb, Log(http.HandlerFunc(GetManualUrlChecksums)), perm.ReadApi, perm.ReadProductData),
		"GET /api/wine-binaries-versions": AuthBearer(sb, Log(http.HandlerFunc(GetWineBinariesVersions)), perm.ReadApi, perm.ReadProductData),
		"GET /api/wine-binary-file":       AuthBearer(sb, Log(http.HandlerFunc(GetWineBinaryFile)), perm.ReadApi, perm.ReadFiles),
		"GET /api/steamcmd-binary-file":   AuthBearer(sb, Log(http.HandlerFunc(GetSteamCmdBinaryFile)), perm.ReadApi, perm.ReadFiles),
		"GET /api/file":                   AuthBearer(sb, Log(http.HandlerFunc(GetFile)), perm.ReadApi, perm.ReadFiles),
		// debug endpoints
		"GET /debug":           AuthCookie(sb, Log(http.HandlerFunc(GetDebug)), perm.ReadDebug),
		"GET /debug-data":      AuthCookie(sb, Log(http.HandlerFunc(GetDebugData)), perm.ReadDebug),
		"GET /logs":            AuthCookie(sb, Log(http.HandlerFunc(GetLogs)), perm.ReadLogs),
		"GET /downloads-queue": AuthCookie(sb, Log(http.HandlerFunc(GetDownloadsQueue)), perm.ReadDebug),
		// cookies import
		"GET /import-cookies":  AuthCookie(sb, Log(http.HandlerFunc(GetImportCookies)), perm.WriteCookies),
		"POST /import-cookies": AuthCookie(sb, Log(http.HandlerFunc(PostImportCookies)), perm.WriteCookies),
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
