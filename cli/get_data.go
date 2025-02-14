package cli

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gog_data"
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

	//data sync is a sequence of specific data types that is required to get
	//all supported data types in one sync session (assuming the connection data
	//is available and the data itself if available, of course)
	//below is a current sequence:
	//
	//- get GOG.com, Steam array and paged data
	//- get GOG.com detail data, PCGamingWiki pageId, steamAppId
	//- reduce PCGamingWiki pageId
	//- get PCGamingWiki externallinks, engine
	//- reduce SteamAppId, HowLongToBeatId, IGDBId
	//- get other detail products (Steam data, HLTB data)
	//- finally, reduce all properties

	////get GOG.com, Steam array and paged data
	//pagedArrayData := append(
	//	vangogh_integration.GOGArrayProducts(),
	//	vangogh_integration.GOGPagedProducts()...)
	//pagedArrayData = append(pagedArrayData,
	//	vangogh_integration.SteamArrayProducts()...)
	//pagedArrayData = append(pagedArrayData,
	//	vangogh_integration.HLTBArrayProducts()...)
	//
	//for _, pt := range pagedArrayData {
	//	if err := GetDataLegacy(nil, nil, pt, since, false, false); err != nil {
	//		return err
	//	}
	//}
	//
	//detailData := vangogh_integration.GOGDetailProducts()
	//detailData = append(detailData, vangogh_integration.PCGWPageId)
	//
	////get GOG.com detail data, PCGamingWiki pageId, steamAppId
	//if err := getDetailData(detailData, since); err != nil {
	//	return err
	//}
	//
	////reduce PCGamingWiki pageId
	//if err := Reduce(since, []string{vangogh_integration.PCGWPageIdProperty}, true); err != nil {
	//	return err
	//}
	//
	////get PCGamingWiki externallinks, engine
	////this needs to happen after reduce, since PCGW PageId - GOG.com ProductId
	////connection is established at reduce from cargo data.
	//pcgwDetailProducts := []vangogh_integration.ProductType{
	//	vangogh_integration.PCGWEngine,
	//	vangogh_integration.PCGWExternalLinks,
	//}
	//
	//if err := getDetailData(pcgwDetailProducts, since); err != nil {
	//	return err
	//}
	//
	////reduce SteamAppId, HowLongToBeatId, IGDBId
	//if err := Reduce(since, []string{
	//	vangogh_integration.SteamAppIdProperty,
	//	vangogh_integration.HLTBBuildIdProperty,
	//	vangogh_integration.HLTBIdProperty,
	//	vangogh_integration.IGDBIdProperty}, true); err != nil {
	//	return err
	//}
	//
	//otherDetailProducts := vangogh_integration.SteamDetailProducts()
	//otherDetailProducts = append(otherDetailProducts, vangogh_integration.HLTBDetailProducts()...)
	//
	////get other detail products (Steam data, HLTB data)
	////this needs to happen after reduce, since Steam AppId - GOG.com ProductId
	////connection is established at reduce. And the earlier data set cannot be retrieved post reduce,
	////since SteamAppList is fetched with initial data
	//if err := getDetailData(otherDetailProducts, since); err != nil {
	//	return err
	//}
	//
	//// finally, reduce all properties
	//if err := Reduce(since, vangogh_integration.ReduxProperties(), false); err != nil {
	//	return err
	//}

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

	if err = gog_data.GetCatalogPages(hc, uat); err != nil {
		return err
	}

	if err = gog_data.GetOrderPages(hc, uat, force); err != nil {
		return err
	}

	if err = gog_data.GetAccountPages(hc, uat, force); err != nil {
		return err
	}

	if err = gog_data.GetApiProducts(hc, uat, since, force); err != nil {
		return err
	}

	if err = gog_data.GetRelatedApiProducts(hc, uat, since, force); err != nil {
		return err
	}

	if err = gog_data.GetDetails(hc, uat); err != nil {
		return err
	}

	if err = gog_data.GetGamesDbProducts(hc, uat, since, force); err != nil {
		return err
	}

	// Steam data

	// PCGW data

	// HLTB data

	// ProtonDB data

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
