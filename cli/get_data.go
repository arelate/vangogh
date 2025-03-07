package cli

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gog_data"
	"github.com/arelate/vangogh/cli/hltb_data"
	"github.com/arelate/vangogh/cli/pcgw_data"
	"github.com/arelate/vangogh/cli/protondb_data"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/arelate/vangogh/cli/steam_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/kevlar"
	"maps"
	"net/http"
	"net/url"
)

func GetDataHandler(u *url.URL) error {

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	force := u.Query().Has("force")
	return GetData(since, force)
}

func GetData(since int64, force bool) error {

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

	if err = gog_data.GetLicences(hc, uat); err != nil {
		return err
	}

	if err = gog_data.GetUserWishlist(hc, uat); err != nil {
		return err
	}

	if err = gog_data.GetCatalogPages(hc, uat, since); err != nil {
		return err
	}

	if err = gog_data.GetAccountPages(hc, uat, since, force); err != nil {
		return err
	}

	if err = gog_data.GetApiProducts(hc, uat, since); err != nil {
		return err
	}

	if err = gog_data.GetRelatedApiProducts(hc, uat, since); err != nil {
		return err
	}

	if err = gog_data.GetOrderPages(hc, uat, since, force); err != nil {
		return err
	}

	if err = gog_data.GetDetails(hc, uat, since); err != nil {
		return err
	}

	if err = gog_data.GetGamesDbGogProducts(hc, uat, since); err != nil {
		return err
	}

	// Steam data

	if err = steam_data.GetAppDetails(since, force); err != nil {
		return err
	}

	catalogAccountProducts, err := shared_data.GetCatalogAccountProducts(since)
	if err != nil {
		return err
	}

	steamGogIds, err := shared_data.GetSteamGogIds(maps.Keys(catalogAccountProducts))
	if err != nil {
		return err
	}

	if err = steam_data.GetAppNews(steamGogIds); err != nil {
		return err
	}

	if err = steam_data.GetAppReviews(steamGogIds, since); err != nil {
		return err
	}

	if err = steam_data.GetDeckCompatibilityReports(steamGogIds, since); err != nil {
		return err
	}

	// PCGamingWiki data

	if err = pcgw_data.GetSteamPageId(steamGogIds, force); err != nil {
		return err
	}

	if err = pcgw_data.GetGogPageId(catalogAccountProducts, force); err != nil {
		return err
	}

	pcgwGogIds, err := shared_data.GetPcgwGogIds(maps.Keys(catalogAccountProducts))
	if err != nil {
		return err
	}

	if err = pcgw_data.GetExternalLinks(pcgwGogIds); err != nil {
		return err
	}

	if err = pcgw_data.GetEngine(pcgwGogIds); err != nil {
		return err
	}

	// HLTB data

	if err = hltb_data.GetRootPage(); err != nil {
		return err
	}

	if err = hltb_data.GetData(since, force); err != nil {
		return err
	}

	// ProtonDB data

	if err = protondb_data.GetSummary(steamGogIds, since); err != nil {
		return err
	}

	// reduce, cascade special properties - has-product-types, owned

	if err = shared_data.ReduceOwned(); err != nil {
		return err
	}

	if err = shared_data.ReduceTypes(); err != nil {
		return err
	}

	return nil
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
