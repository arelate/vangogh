package cli

import (
	"encoding/json/v2"
	"errors"
	"maps"
	"net/http"
	"net/url"
	"os"
	"slices"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gamesdb_data"
	"github.com/arelate/vangogh/cli/gog_data"
	"github.com/arelate/vangogh/cli/hltb_data"
	"github.com/arelate/vangogh/cli/opencritic_data"
	"github.com/arelate/vangogh/cli/pcgw_data"
	"github.com/arelate/vangogh/cli/protondb_data"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/arelate/vangogh/cli/steam_data"
	"github.com/arelate/vangogh/cli/wikipedia_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

type dataFilter struct {
	purchases          bool
	extraData          bool
	relatedApiProducts bool
}

var gogUserAccessTokenTypes = []vangogh_integration.ProductType{
	vangogh_integration.GogLicences,
	vangogh_integration.GogUserWishlist,
	vangogh_integration.GogCatalogPage,
	vangogh_integration.GogAccountPage,
	vangogh_integration.GogApiProducts,
	vangogh_integration.GogDetails,
	vangogh_integration.GamesDbGogProducts,
}

var gogCatalogAccountGogIdsProductTypes = []vangogh_integration.ProductType{
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
	vangogh_integration.PcgwRaw,
}

var wikipediaProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.WikipediaRaw,
}

var hltbProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.HltbData,
}

var openCriticIdsProductTypes = []vangogh_integration.ProductType{
	vangogh_integration.OpenCriticApiGame,
}

func GetDataHandler(u *url.URL) error {

	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	productTypes := vangogh_integration.ProductTypesFromUrl(u)

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	q := u.Query()

	df := new(dataFilter{
		purchases:          q.Has("purchases"),
		extraData:          q.Has("extra"),
		relatedApiProducts: q.Has("related-api-products"),
	})

	force := q.Has("force")
	return GetData(ids, productTypes, since, df, force)
}

func GetData(ids []string, productTypes []vangogh_integration.ProductType, since int64, dataFilter *dataFilter, force bool) error {

	if dataFilter.purchases {
		productTypes = append(productTypes, vangogh_integration.GogPurchaseProductTypes()...)
	}
	if dataFilter.extraData {
		productTypes = append(productTypes, slices.Collect(vangogh_integration.ExtraProductTypes())...)
	}

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

	var uat string
	if requiresGogUserAccessToken(productTypes...) {

		if err = gog_data.GetGogUserAccessToken(hc); err != nil {
			nod.LogError(err)
		} else {
			uat, err = readGogUserAccessToken()
			if err != nil {
				return err
			}
		}

		if hc.Jar != nil {
			if err = coost.Write(hc.Jar, gog_integration.HostUrl(), vangogh_integration.AbsCookiesPath()); err != nil {
				return err
			}
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogLicences) {
		if err = gog_data.GetGogLicences(hc, uat); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogUserWishlist) {
		if err = gog_data.GetGogUserWishlist(hc, uat); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogCatalogPage) {
		if err = gog_data.GetGogCatalogPages(hc, uat, since); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogAccountPage) {
		if err = gog_data.GetGogAccountPages(hc, uat, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogApiProducts) {
		if err = gog_data.GetGogApiProducts(ids, hc, uat, since, force); err != nil {
			return err
		}
		if dataFilter.relatedApiProducts {
			if err = gog_data.GetRelatedGogApiProducts(hc, uat, since, force); err != nil {
				return err
			}
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogOrderPage) {
		if err = gog_data.GetGogOrderPages(hc, uat, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GogDetails) {
		if err = gog_data.GetGogDetails(ids, hc, uat, since, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.GamesDbGogProducts) {
		if err = gamesdb_data.GetGamesDbGogProducts(ids, hc, uat, since, force); err != nil {
			return err
		}
	}

	// Steam data

	var catalogAccountProducts map[string]any
	if len(ids) > 0 {
		catalogAccountProducts = make(map[string]any)
		for _, id := range ids {
			catalogAccountProducts[id] = nil
		}
	} else if requiresCatalogAccountGogIds(productTypes...) {
		// fetch.Items will check if certain data type should be updated, so
		// we shouldn't limit catalog-products and account-products only to
		// the recently updated, instead passing all of them and let
		// last updated check figure what needs to be updated
		catalogAccountProducts, err = shared_data.GetGogCatalogAccountProducts(-1)
		if err != nil {
			return err
		}
	}

	catalogAccountGames, err := shared_data.GetGameGogIds(catalogAccountProducts)
	if err != nil {
		return err
	}

	var steamGogIds map[string][]string
	if requiresSteamAppIds(productTypes...) {
		steamGogIds, err = shared_data.GetSteamGogIds(maps.Keys(catalogAccountGames))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamAppDetails) {
		if err = steam_data.GetAppDetails(steamGogIds, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamAppNews) {
		if err = steam_data.GetAppNews(steamGogIds, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamAppReviews) {
		if err = steam_data.GetAppReviews(steamGogIds, force); err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.SteamDeckCompatibilityReport) {
		if err = steam_data.GetDeckCompatibilityReports(steamGogIds, force); err != nil {
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
		if err = pcgw_data.GetGogPageId(catalogAccountGames, force); err != nil {
			return err
		}
	}

	var pcgwGogIds map[string][]string
	if requiresPcgwPageIds(productTypes...) {
		pcgwGogIds, err = shared_data.GetPcgwGogIds(maps.Keys(catalogAccountGames))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.PcgwRaw) {
		if err = pcgw_data.GetRaw(pcgwGogIds, force); err != nil {
			return err
		}
	}

	// Wikipedia data

	var wikipediaIds map[string][]string
	if requiresWikipediaIds(productTypes...) {
		wikipediaIds, err = shared_data.GetWikipediaIds(maps.Keys(catalogAccountGames))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.WikipediaRaw) {
		if err = wikipedia_data.GetRaw(wikipediaIds, force); err != nil {
			return err
		}
	}

	// HLTB data

	var hltbGogIds map[string][]string
	if requiresHltbIds(productTypes...) {
		hltbGogIds, err = shared_data.GetHltbIds(maps.Keys(catalogAccountGames))
		if err != nil {
			return err
		}
	}

	if slices.Contains(productTypes, vangogh_integration.HltbData) {
		if err = hltb_data.GetRootPage(); err != nil {
			return err
		}

		if err = hltb_data.GetData(hltbGogIds, force); err != nil {
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

	return nil
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

func requiresWikipediaIds(productTypes ...vangogh_integration.ProductType) bool {
	for _, wpt := range wikipediaProductTypes {
		if slices.Contains(productTypes, wpt) {
			return true
		}
	}
	return false
}

func requiresHltbIds(productTypes ...vangogh_integration.ProductType) bool {
	for _, hpt := range hltbProductTypes {
		if slices.Contains(productTypes, hpt) {
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
	if requiresWikipediaIds(productTypes...) {
		return true
	}
	if requiresHltbIds(productTypes...) {
		return true
	}
	if requiresOpenCriticIds(productTypes...) {
		return true
	}
	for _, cagpt := range gogCatalogAccountGogIdsProductTypes {
		if slices.Contains(productTypes, cagpt) {
			return true
		}
	}
	return false
}

func requiresGogUserAccessToken(productTypes ...vangogh_integration.ProductType) bool {
	for _, uatt := range gogUserAccessTokenTypes {
		if slices.Contains(productTypes, uatt) {
			return true
		}
	}
	return false
}

func gogAuthHttpClient() (*http.Client, error) {
	acp := vangogh_integration.AbsCookiesPath()

	jar, err := coost.Read(gog_integration.HostUrl(), acp)
	if os.IsNotExist(err) {
		return nil, errors.New("cookies file not found, use import-cookies command to add")
	} else if err != nil {
		return nil, err
	}

	hc := http.DefaultClient
	hc.Jar = jar

	if err = gog_integration.IsLoggedIn(hc); err != nil {
		return nil, err
	}

	return hc, nil
}

func readGogUserAccessToken() (string, error) {
	gogUatDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogUserAccessToken)
	if err != nil {
		return "", err
	}

	kvGogUat, err := kevlar.New(gogUatDir, kevlar.JsonExt)
	if err != nil {
		return "", err
	}

	rcGogUat, err := kvGogUat.Get(vangogh_integration.GogUserAccessToken.String())
	if err != nil {
		return "", err
	}
	defer rcGogUat.Close()

	var uat gog_integration.UserAccessToken

	if err = json.UnmarshalRead(rcGogUat, &uat); err != nil {
		return "", err
	}

	return uat.AccessToken, nil
}
