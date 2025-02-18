package cli

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/steam_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/kevlar"
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

	// GOG.com data

	//hc, err := gogAuthHttpClient()
	//if err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetUserAccessToken(hc); err != nil {
	//	return err
	//}
	//
	//uat, err := readUserAccessToken()
	//if err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetLicences(hc, uat); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetUserWishlist(hc, uat); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetCatalogPages(hc, uat, since); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetOrderPages(hc, uat, since, force); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetAccountPages(hc, uat, since, force); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetApiProducts(hc, uat, since, force); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetRelatedApiProducts(hc, uat, since, force); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetDetails(hc, uat, since); err != nil {
	//	return err
	//}
	//
	//if err = gog_data.GetGamesDbGogProducts(hc, uat, since, force); err != nil {
	//	return err
	//}

	// Steam data

	if err = steam_data.GetAppDetails(since, force); err != nil {
		return err
	}

	// PCGW data

	// HLTB data

	// ProtonDB data

	// reduce, cascade special properties - owned, validation-status, etc - or should this be done in runtime?

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
