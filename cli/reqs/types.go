package reqs

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/protondb_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"net/http"
	"net/url"
)

const (
	defaultRateLimitRequests = 200
	defaultRateLimitSeconds  = 5
)

type Params struct {
	ProductType       vangogh_integration.ProductType
	UrlFunc           func(id string) *url.URL
	HttpClient        *http.Client
	HttpMethod        string
	AuthBearer        string
	RateLimitRequests float64
	RateLimitSeconds  float64
}

func UserAccessToken(authHttpClient *http.Client) *Params {
	return &Params{
		HttpClient: authHttpClient,
		HttpMethod: http.MethodPost,
	}
}

func Licenses(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		HttpClient: authHttpClient,
		HttpMethod: http.MethodGet,
		AuthBearer: authBearer,
	}
}

func UserWishlist(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		HttpClient: authHttpClient,
		HttpMethod: http.MethodGet,
		AuthBearer: authBearer,
	}
}

func AccountPage(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.AccountPage,
		UrlFunc:     gog_integration.AccountPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func CatalogPage(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.CatalogPage,
		UrlFunc:     gog_integration.CatalogPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func OrderPage(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.OrderPage,
		UrlFunc:     gog_integration.OrdersPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func ApiProducts(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.ApiProductsV2,
		UrlFunc:     gog_integration.ApiProductV2Url,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func Details(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.Details,
		UrlFunc:     gog_integration.DetailsUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func GamesDbGogProduct(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.GamesDbGogProducts,
		UrlFunc:     gog_integration.GamesDbGogExternalReleaseUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
	}
}

func SteamAppDetails() *Params {
	return &Params{
		ProductType:       vangogh_integration.SteamAppDetails,
		UrlFunc:           steam_integration.AppDetailsUrl,
		HttpClient:        http.DefaultClient,
		HttpMethod:        http.MethodGet,
		RateLimitSeconds:  defaultRateLimitSeconds,
		RateLimitRequests: defaultRateLimitRequests,
	}
}

func SteamAppNews() *Params {
	return &Params{
		ProductType: vangogh_integration.SteamAppNews,
		UrlFunc:     steam_integration.NewsForAppUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func SteamAppReviews() *Params {
	return &Params{
		ProductType: vangogh_integration.SteamAppReviews,
		UrlFunc:     steam_integration.AppReviewsUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func SteamDeckCompatibilityReports() *Params {
	return &Params{
		ProductType: vangogh_integration.SteamDeckCompatibilityReport,
		UrlFunc:     steam_integration.DeckAppCompatibilityReportUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func ProtonDbSummary() *Params {
	return &Params{
		ProductType: vangogh_integration.ProtonDBSummary,
		UrlFunc:     protondb_integration.SummaryUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}
