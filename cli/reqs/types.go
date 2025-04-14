package reqs

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/opencritic_integration"
	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/protondb_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"net/http"
	"net/url"
)

const (
	defaultRateLimitRequests = 200
	defaultRateLimitSeconds  = 5
)

var (
	GitTag string
)

const (
	safariUserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.3 Safari/605.1.15"
)

type Params struct {
	ProductType       vangogh_integration.ProductType
	UrlFunc           func(id string) *url.URL
	HttpClient        *http.Client
	HttpMethod        string
	AuthBearer        string
	UserAgent         string
	RateLimitRequests float64
	RateLimitSeconds  float64
}

func GetDefaultUserAgent() string {
	return "vangogh " + GitTag
}

func GetDoloClient() *dolo.Client {
	return dolo.NewClient(http.DefaultClient, &dolo.ClientOptions{
		UserAgent:          GetDefaultUserAgent(),
		CheckContentLength: false,
		ResumeDownloads:    true,
	})
}

func UserAccessToken(authHttpClient *http.Client) *Params {
	return &Params{
		ProductType: vangogh_integration.UserAccessToken,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodPost,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func Licenses(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.Licences,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func UserWishlist(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.UserWishlist,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func AccountPage(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.AccountPage,
		UrlFunc:     gog_integration.AccountPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func CatalogPage(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.CatalogPage,
		UrlFunc:     gog_integration.CatalogPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func OrderPage(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.OrderPage,
		UrlFunc:     gog_integration.OrdersPageUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func ApiProducts(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.ApiProducts,
		UrlFunc:     gog_integration.ApiProductUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func Details(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.Details,
		UrlFunc:     gog_integration.DetailsUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
	}
}

func GamesDbGogProduct(authHttpClient *http.Client, authBearer string) *Params {
	return &Params{
		ProductType: vangogh_integration.GamesDbGogProducts,
		UrlFunc:     gog_integration.GamesDbGogExternalReleaseUrl,
		HttpClient:  authHttpClient,
		HttpMethod:  http.MethodGet,
		AuthBearer:  authBearer,
		UserAgent:   GetDefaultUserAgent(),
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
		ProductType: vangogh_integration.ProtonDbSummary,
		UrlFunc:     protondb_integration.SummaryUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func PcgwSteamPageId() *Params {
	return &Params{
		ProductType: vangogh_integration.PcgwSteamPageId,
		UrlFunc:     pcgw_integration.SteamPageIdCargoQueryUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func PcgwGogPageId() *Params {
	return &Params{
		ProductType: vangogh_integration.PcgwGogPageId,
		UrlFunc:     pcgw_integration.GogPageIdCargoQueryUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func PcgwExternalLinks() *Params {
	return &Params{
		ProductType: vangogh_integration.PcgwExternalLinks,
		UrlFunc:     pcgw_integration.ParseExternalLinksUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func PcgwEngine() *Params {
	return &Params{
		ProductType: vangogh_integration.PcgwEngine,
		UrlFunc:     pcgw_integration.EngineCargoQueryUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

func PcgwRaw() *Params {
	return &Params{
		ProductType: vangogh_integration.PcgwRaw,
		UrlFunc:     pcgw_integration.WikiRawUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
		UserAgent:   safariUserAgent,
	}
}

func HltbRootPage() *Params {
	return &Params{
		ProductType: vangogh_integration.HltbRootPage,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
		UserAgent:   safariUserAgent,
	}
}

func OpenCriticApiGame() *Params {
	return &Params{
		ProductType: vangogh_integration.OpenCriticApiGame,
		UrlFunc:     opencritic_integration.ApiGameUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
	}
}

type hltbBuilder struct {
	buildId string
}

func (hb *hltbBuilder) DataUrl(hltbId string) *url.URL {
	return hltb_integration.DataUrl(hb.buildId, hltbId)
}

func HltbData(buildId string) *Params {

	hb := &hltbBuilder{buildId: buildId}

	return &Params{
		ProductType: vangogh_integration.HltbData,
		UrlFunc:     hb.DataUrl,
		HttpClient:  http.DefaultClient,
		HttpMethod:  http.MethodGet,
		UserAgent:   safariUserAgent,
	}
}
