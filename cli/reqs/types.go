package reqs

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"net/http"
	"net/url"
)

const (
	defaultRateLimitRequests = 200
	defaultRateLimitSeconds  = 5
)

type Builder struct {
	ProductType       vangogh_integration.ProductType
	UrlFunc           func(id string) *url.URL
	HttpClient        *http.Client
	HttpMethod        string
	AuthBearer        string
	RateLimitRequests int
	RateLimitSeconds  int
}

func UserAccessToken(authHttpClient *http.Client) *Builder {
	return &Builder{
		HttpClient: authHttpClient,
		HttpMethod: http.MethodPost,
	}
}

func Licenses(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		HttpClient: authHttpClient,
		HttpMethod: http.MethodGet,
		AuthBearer: authBearer,
	}
}

func UserWishlist(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		HttpClient: authHttpClient,
		HttpMethod: http.MethodGet,
		AuthBearer: authBearer,
	}
}

func AccountPage(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		ProductType: vangogh_integration.AccountPage,
		UrlFunc:     gog_integration.AccountPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func CatalogPage(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		ProductType: vangogh_integration.CatalogPage,
		UrlFunc:     gog_integration.CatalogPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func OrderPage(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		ProductType: vangogh_integration.OrderPage,
		UrlFunc:     gog_integration.OrdersPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func ApiProducts(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		ProductType:       vangogh_integration.ApiProductsV2,
		UrlFunc:           gog_integration.ApiProductV2Url,
		HttpClient:        authHttpClient,
		HttpMethod:        http.MethodGet,
		AuthBearer:        authBearer,
		RateLimitSeconds:  defaultRateLimitSeconds,
		RateLimitRequests: defaultRateLimitRequests,
	}
}

func Details(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		ProductType:       vangogh_integration.Details,
		UrlFunc:           gog_integration.DetailsUrl,
		HttpClient:        authHttpClient,
		HttpMethod:        http.MethodGet,
		AuthBearer:        authBearer,
		RateLimitSeconds:  defaultRateLimitSeconds,
		RateLimitRequests: defaultRateLimitRequests,
	}
}

func GamesDbGogProduct(authHttpClient *http.Client, authBearer string) *Builder {
	return &Builder{
		ProductType: vangogh_integration.GamesDbGogProducts,
		UrlFunc:     gog_integration.GamesDbGogExternalReleaseUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func SteamAppDetails() *Builder {
	return &Builder{
		UrlFunc:           steam_integration.AppDetailsUrl,
		HttpClient:        http.DefaultClient,
		HttpMethod:        http.MethodGet,
		AuthBearer:        "",
		RateLimitSeconds:  defaultRateLimitSeconds,
		RateLimitRequests: defaultRateLimitRequests,
	}
}
