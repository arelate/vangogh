package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gog_data"
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

	if err := gog_data.GetLicences(); err != nil {
		return err
	}
	if err := gog_data.GetUserWishlist(); err != nil {
		return err
	}

	if err := gog_data.GetCatalogPages(); err != nil {
		return err
	}

	if err := gog_data.GetOrderPages(force); err != nil {
		return err
	}

	if err := gog_data.GetAccountPages(force); err != nil {
		return err
	}

	if err := gog_data.GetApiProducts(since, force); err != nil {
		return err
	}

	if err := gog_data.GetRelatedApiProducts(since, force); err != nil {
		return err
	}

	if err := gog_data.GetDetails(); err != nil {
		return err
	}

	return nil
}
