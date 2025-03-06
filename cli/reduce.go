package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gog_data"
	"github.com/arelate/vangogh/cli/hltb_data"
	"github.com/arelate/vangogh/cli/pcgw_data"
	"github.com/arelate/vangogh/cli/protondb_data"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/arelate/vangogh/cli/steam_data"
	"github.com/boggydigital/kevlar"
	"maps"
	"net/url"
)

func ReduceHandler(_ *url.URL) error {
	return Reduce()
}

func Reduce() error {

	productTypes := []vangogh_integration.ProductType{
		// GOG
		vangogh_integration.Licences,
		vangogh_integration.UserWishlist,
		vangogh_integration.CatalogPage,
		vangogh_integration.OrderPage,
		vangogh_integration.AccountPage,
		vangogh_integration.ApiProducts,
		vangogh_integration.Details,
		vangogh_integration.GamesDbGogProducts,
		// Steam
		vangogh_integration.SteamAppDetails,
		vangogh_integration.SteamAppNews,
		vangogh_integration.SteamAppReviews,
		vangogh_integration.SteamDeckCompatibilityReport,
		// HLTB
		vangogh_integration.HltbData,
		// ProtonDB
		vangogh_integration.ProtonDbSummary,
	}

	for _, pt := range productTypes {
		if err := reduceProductType(pt); err != nil {
			return err
		}
	}

	//if err := reducePcgwData(); err != nil {
	//	return err
	//}

	if err := shared_data.ReduceOwned(); err != nil {
		return err
	}

	if err := shared_data.ReduceTypes(); err != nil {
		return err
	}

	return nil
}

func reduceProductType(pt vangogh_integration.ProductType) error {

	ptDir, err := vangogh_integration.AbsProductTypeDir(pt)
	if err != nil {
		return err
	}

	kvPt, err := kevlar.New(ptDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	switch pt {
	case vangogh_integration.Licences:
		return gog_data.ReduceLicences(kvPt)
	case vangogh_integration.UserWishlist:
		return gog_data.ReduceUserWishlist(kvPt)
	case vangogh_integration.CatalogPage:
		return gog_data.ReduceCatalogPages(kvPt, -1)
	case vangogh_integration.OrderPage:
		return gog_data.ReduceOrderPages(kvPt, -1)
	case vangogh_integration.AccountPage:
		return gog_data.ReduceAccountPages(kvPt, -1)
	case vangogh_integration.ApiProducts:
		return gog_data.ReduceApiProducts(kvPt, -1)
	case vangogh_integration.Details:
		return gog_data.ReduceDetails(kvPt, -1)
	case vangogh_integration.GamesDbGogProducts:
		return gog_data.ReduceGamesDbGogProducts(kvPt, -1)
	case vangogh_integration.SteamAppDetails:
		return steam_data.ReduceAppDetails(kvPt, -1)
	case vangogh_integration.SteamAppNews:
		// do nothing
	case vangogh_integration.SteamAppReviews:
		return steam_data.ReduceAppReviews(kvPt, -1)
	case vangogh_integration.SteamDeckCompatibilityReport:
		return steam_data.ReduceDeckCompatibilityReports(kvPt, -1)
	case vangogh_integration.HltbData:
		return hltb_data.ReduceData(kvPt, -1)
	case vangogh_integration.ProtonDbSummary:
		return protondb_data.ReduceSummary(kvPt, -1)
	}

	return nil
}

func reducePcgwData() error {

	catalogAccountProducts, err := shared_data.GetCatalogAccountProducts(-1)
	if err != nil {
		return err
	}

	steamGogIds, err := shared_data.GetSteamGogIds(maps.Keys(catalogAccountProducts))
	if err != nil {
		return err
	}

	gameSteamGogIds, err := pcgw_data.GetGameSteamGogIds(steamGogIds)
	if err != nil {
		return err
	}

	steamPageIdDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwSteamPageId)
	if err != nil {
		return err
	}

	kvSteamPageId, err := kevlar.New(steamPageIdDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	newGameSteamGogIds := pcgw_data.GetNewSteamGogIds(gameSteamGogIds, kvSteamPageId, true)

	if err = pcgw_data.ReduceSteamPageIds(newGameSteamGogIds, kvSteamPageId); err != nil {
		return err
	}

	gogPageIdDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwGogPageId)
	if err != nil {
		return err
	}

	kvGogPageId, err := kevlar.New(gogPageIdDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gameGogIds, err := pcgw_data.GetGameGogIds(catalogAccountProducts)
	if err != nil {
		return err
	}

	newGameGogIds := pcgw_data.GetNewGogIds(gameGogIds, kvGogPageId, true)

	if err = pcgw_data.ReduceGogPageIds(newGameGogIds, kvGogPageId); err != nil {
		return nil
	}

	pcgwGogIds, err := shared_data.GetPcgwGogIds(maps.Keys(catalogAccountProducts))
	if err != nil {
		return err
	}

	externalLinksDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwExternalLinks)
	if err != nil {
		return err
	}

	kvExternalLinks, err := kevlar.New(externalLinksDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = pcgw_data.ReduceExternalLinks(pcgwGogIds, kvExternalLinks); err != nil {
		return err
	}

	engineDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwEngine)
	if err != nil {
		return err
	}

	kvEngine, err := kevlar.New(engineDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	return pcgw_data.ReduceEngine(pcgwGogIds, kvEngine)
}
