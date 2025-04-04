package cli

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gog_data"
	"github.com/arelate/vangogh/cli/hltb_data"
	"github.com/arelate/vangogh/cli/opencritic_data"
	"github.com/arelate/vangogh/cli/pcgw_data"
	"github.com/arelate/vangogh/cli/protondb_data"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/arelate/vangogh/cli/steam_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/kevlar"
	"maps"
	"net/http"
	"net/url"
	"slices"
)

var catalogAccountGogIdsProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.PcgwGogPageId,
}

var steamAppIdsProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.SteamAppDetails,
	vangogh_integration.SteamAppNews,
	vangogh_integration.SteamAppReviews,
	vangogh_integration.SteamDeckCompatibilityReport,
	vangogh_integration.PcgwSteamPageId,
	vangogh_integration.ProtonDbSummary,
}

var pcgwPageIdsProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.PcgwExternalLinks,
	vangogh_integration.PcgwEngine,
	vangogh_integration.PcgwRaw,
}

var openCriticIdsProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.OpenCriticApiGame,
}

func GetDataHandler(u *url.URL) error {

	productTypes := vangogh_integration.ProductTypesFromUrl(u)

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	force := u.Query().Has("force")
	return GetData(productTypes, since, force)
}

func GetData(productTypes []vangogh_integration.ProductType, since int64, force bool) error {

	if len(productTypes) == 0 {
		productTypes = slices.Collect(vangogh_integration.AllProductTypes())
	}

	var err error

	if force {
		since = -1
	}

	// GOG.com data

	hc, err := gogAuthHttpClient()
	if err != nil {
		return err
	}

	if err = gog_data.GetUserAccessToken(hc); err != nil {
		return err
	}

	uat, err := readUserAccessToken()
	if err != nil {
		return err
	}

	if slices.Contains(productTypes, vangogh_integration.Licences) {
		if err = gog_data.GetLicences(hc, uat); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.UserWishlist) {
		if err = gog_data.GetUserWishlist(hc, uat); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.CatalogPage) {
		if err = gog_data.GetCatalogPages(hc, uat, since); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.AccountPage) {
		if err = gog_data.GetAccountPages(hc, uat, since); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.ApiProducts) {
		if err = gog_data.GetApiProducts(hc, uat, since, force); err != nil {
			return err
		}
		if err = gog_data.GetRelatedApiProducts(hc, uat, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.OrderPage) {
		if err = gog_data.GetOrderPages(hc, uat, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.Details) {
		if err = gog_data.GetDetails(hc, uat, since); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GamesDbGogProducts) {
		if err = gog_data.GetGamesDbGogProducts(hc, uat, since, force); err != nil {
			return err
		}
	}

	// Steam data

	var catalogAccountProducts map[string]any
	if requiresCatalogAccountGogIds(productTypes...) {
		catalogAccountProducts, err = shared_data.GetCatalogAccountProducts(since)
		if err != nil {
			return err
		}
	}

	var steamGogIds map[string][]string
	if requiresSteamAppIds(productTypes...) {
		steamGogIds, err = shared_data.GetSteamGogIds(maps.Keys(catalogAccountProducts))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamAppDetails) {
		if err = steam_data.GetAppDetails(steamGogIds, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamAppNews) {
		if err = steam_data.GetAppNews(steamGogIds); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamAppReviews) {
		if err = steam_data.GetAppReviews(steamGogIds, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamDeckCompatibilityReport) {
		if err = steam_data.GetDeckCompatibilityReports(steamGogIds, since, force); err != nil {
			return err
		}
	}

	// PCGamingWiki data

	if slices.Contains(productTypes, vangogh_integration.PcgwSteamPageId) {
		if err = pcgw_data.GetSteamPageId(steamGogIds, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.PcgwGogPageId) {
		if err = pcgw_data.GetGogPageId(catalogAccountProducts, force); err != nil {
			return err
		}
	}

	var pcgwGogIds map[string][]string
	if requiresPcgwPageIds(productTypes...) {
		pcgwGogIds, err = shared_data.GetPcgwGogIds(maps.Keys(catalogAccountProducts))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.PcgwExternalLinks) {
		if err = pcgw_data.GetExternalLinks(pcgwGogIds, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.PcgwEngine) {
		if err = pcgw_data.GetEngine(pcgwGogIds, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.PcgwRaw) {
		if err = pcgw_data.GetRaw(pcgwGogIds, force); err != nil {
			return err
		}
	}

	// HLTB data

	if slices.Contains(productTypes, vangogh_integration.HltbData) {
		if err = hltb_data.GetRootPage(); err != nil {
			return err
		}
		if err = hltb_data.GetData(since, force); err != nil {
			return err
		}
	}

	// ProtonDB data

	if slices.Contains(productTypes, vangogh_integration.ProtonDbSummary) {
		if err = protondb_data.GetSummary(steamGogIds, since, force); err != nil {
			return err
		}
	}

	var openCriticGogIds map[string][]string
	if requiresOpenCriticIds(productTypes...) {
		catalogAccountGames, err := shared_data.GetGameGogIds(catalogAccountProducts)
		if err != nil {
			return err
		}
		openCriticGogIds, err = shared_data.GetOpenCriticGogIds(maps.Keys(catalogAccountGames))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.OpenCriticApiGame) {
		if err = opencritic_data.GetApiGame(openCriticGogIds, force); err != nil {
			return err
		}
	}

	return shared_data.ReduceMisc()
}

func requiresSteamAppIds(productTypes ...vangogh_integration.ProductType) bool {
	for _, spt := range steamAppIdsProductTypes {
		if slices.Contains(productTypes, spt) {
			return true
		}
	}
	return false
}

func requiresPcgwPageIds(productTypes ...vangogh_integration.ProductType) bool {
	for _, ppt := range pcgwPageIdsProductTypes {
		if slices.Contains(productTypes, ppt) {
			return true
		}
	}
	return false
}

func requiresOpenCriticIds(productTypes ...vangogh_integration.ProductType) bool {
	for _, opt := range openCriticIdsProductTypes {
		if slices.Contains(productTypes, opt) {
			return true
		}
	}
	return false
}

func requiresCatalogAccountGogIds(productTypes ...vangogh_integration.ProductType) bool {
	if requiresSteamAppIds(productTypes...) {
		return true
	}
	if requiresPcgwPageIds(productTypes...) {
		return true
	}
	if requiresOpenCriticIds(productTypes...) {
		return true
	}
	for _, cagpt := range catalogAccountGogIdsProductTypes {
		if slices.Contains(productTypes, cagpt) {
			return true
		}
	}
	return false
}

func gogAuthHttpClient() (*http.Client, error) {
	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return nil, err
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return nil, err
	}

	if err = gog_integration.IsLoggedIn(hc); err != nil {
		return nil, err
	}

	return hc, nil
}

func readUserAccessToken() (string, error) {
	userAccessTokenDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.UserAccessToken)
	if err != nil {
		return "", err
	}

	kvUserAccessToken, err := kevlar.New(userAccessTokenDir, kevlar.JsonExt)
	if err != nil {
		return "", err
	}

	rcUserAccessToken, err := kvUserAccessToken.Get(vangogh_integration.UserAccessToken.String())
	if err != nil {
		return "", err
	}
	defer rcUserAccessToken.Close()

	var uat gog_integration.UserAccessToken

	if err = json.NewDecoder(rcUserAccessToken).Decode(&uat); err != nil {
		return "", err
	}

	return uat.AccessToken, nil
}
