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
		"GET /updates":                AuthCookie(sb, Log(http.HandlerFunc(GetUpdates)), perm.ReadUpdates),
		"GET /search":                 AuthCookie(sb, Log(http.HandlerFunc(GetSearch)), perm.ReadSearch),
		"GET /gog-product/{id}":       AuthCookie(sb, Log(http.HandlerFunc(GetGogProduct)), perm.ReadProductData),
		"GET /gog-slug/{slug}":        AuthCookie(sb, Log(http.HandlerFunc(GetGogSlug)), perm.ReadProductData),
		"GET /gog-info/{id}":          AuthCookie(sb, Log(http.HandlerFunc(GetGogInfo)), perm.ReadProductData),
		"GET /gog-description/{id}":   AuthCookie(sb, Log(http.HandlerFunc(GetGogDescription)), perm.ReadProductData),
		"GET /gog-reception/{id}":     AuthCookie(sb, Log(http.HandlerFunc(GetGogReception)), perm.ReadProductData),
		"GET /gog-offerings/{id}":     AuthCookie(sb, Log(http.HandlerFunc(GetGogOfferings)), perm.ReadProductData),
		"GET /gog-media/{id}":         AuthCookie(sb, Log(http.HandlerFunc(GetGogMedia)), perm.ReadProductData),
		"GET /gog-news/{id}":          AuthCookie(sb, Log(http.HandlerFunc(GetGogNews)), perm.ReadProductData),
		"GET /gog-news/all/{id}":      AuthCookie(sb, Log(http.HandlerFunc(GetGogNews)), perm.ReadProductData),
		"GET /gog-changelog/{id}":     AuthCookie(sb, Log(http.HandlerFunc(GetGogChangelog)), perm.ReadProductData),
		"GET /gog-compatibility/{id}": AuthCookie(sb, Log(http.HandlerFunc(GetGogCompatibility)), perm.ReadProductData),
		"GET /gog-installers/{id}":    AuthCookie(sb, Log(http.HandlerFunc(GetGogInstallers)), perm.ReadFiles),
		"GET /binaries":               AuthCookie(sb, Log(http.HandlerFunc(GetBinaries)), perm.ReadFiles),
		// public media endpoints
		"GET /gog-image/{imageId}":            AuthCookie(sb, Log(http.HandlerFunc(GetGogImage)), perm.ReadImages),
		"GET /description-image/{relPath...}": AuthCookie(sb, Log(http.HandlerFunc(GetDescriptionImage)), perm.ReadImages),
		// binaries endpoints
		"GET /binary/{os}/{title...}": AuthCookie(sb, Log(http.HandlerFunc(GetBinary)), perm.ReadFiles),
		// auth data endpoints
		"GET /wishlist/add":     AuthCookie(sb, Log(http.HandlerFunc(GetWishlistAdd)), perm.WriteWishlist),
		"GET /wishlist/remove":  AuthCookie(sb, Log(http.HandlerFunc(GetWishlistRemove)), perm.WriteWishlist),
		"GET /tags/edit":        AuthCookie(sb, Log(http.HandlerFunc(GetTagsEdit)), perm.WriteTagId),
		"GET /tags/apply":       AuthCookie(sb, Log(http.HandlerFunc(GetTagsApply)), perm.WriteTagId),
		"GET /local-tags/edit":  AuthCookie(sb, Log(http.HandlerFunc(GetLocalTagsEdit)), perm.WriteLocalTags),
		"GET /local-tags/apply": AuthCookie(sb, Log(http.HandlerFunc(GetLocalTagsApply)), perm.WriteLocalTags),
		// auth files endpoints
		"GET /gog-manual-url/{id}/{dt}/{mu...}": AuthCookie(sb, Log(http.HandlerFunc(GetGogManualUrl)), perm.ReadFiles),
		// products redirects
		"GET /products": Redirect("/search", http.StatusPermanentRedirect),
		// API
		"POST /api/auth-user":                       Log(http.HandlerFunc(sb.AuthApiUsernamePassword)),
		"POST /api/auth-session":                    Log(http.HandlerFunc(sb.AuthApiSession)),
		"GET /api/available-products":               AuthBearer(sb, Log(http.HandlerFunc(GetAvailableProducts)), perm.ReadApi, perm.ReadProductData),
		"GET /api/gog-checksums/{id}":               AuthBearer(sb, Log(http.HandlerFunc(GetGogChecksums)), perm.ReadApi, perm.ReadProductData),
		"GET /api/gog-filenames/{id}":               AuthBearer(sb, Log(http.HandlerFunc(GetGogFilenames)), perm.ReadApi, perm.ReadProductData),
		"GET /api/binaries/versions":                AuthBearer(sb, Log(http.HandlerFunc(GetBinariesVersions)), perm.ReadApi, perm.ReadProductData),
		"GET /api/binary/{os}/{title...}":           AuthBearer(sb, Log(http.HandlerFunc(GetBinary)), perm.ReadApi, perm.ReadFiles),
		"GET /api/gog-manual-url/{id}/{dt}/{mu...}": AuthBearer(sb, Log(http.HandlerFunc(GetGogManualUrl)), perm.ReadApi, perm.ReadFiles),
		"GET /api/gog-image/{imageId}":              AuthBearer(sb, Log(http.HandlerFunc(GetGogImage)), perm.ReadApi, perm.ReadImages),
		"GET /api/metadata/{productType}/{id}":      AuthBearer(sb, Log(http.HandlerFunc(GetMetadata)), perm.ReadApi, perm.ReadProductData),
		// debug endpoints
		"GET /debug/{id}":                    AuthCookie(sb, Log(http.HandlerFunc(GetDebug)), perm.ReadDebug),
		"GET /debug-data/{productType}/{id}": AuthCookie(sb, Log(http.HandlerFunc(GetDebugData)), perm.ReadDebug),
		"GET /logs":                          AuthCookie(sb, Log(http.HandlerFunc(GetLogs)), perm.ReadLogs),
		"GET /log/{logId}":                   AuthCookie(sb, Log(http.HandlerFunc(GetLog)), perm.ReadLogs),
		"GET /downloads-queue":               AuthCookie(sb, Log(http.HandlerFunc(GetDownloadsQueue)), perm.ReadDebug),
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
