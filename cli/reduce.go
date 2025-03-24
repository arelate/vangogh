package cli

import (
	"errors"
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
	"slices"
)

func ReduceHandler(u *url.URL) error {

	productTypes := vangogh_integration.ProductTypesFromUrl(u)

	return Reduce(productTypes)
}

func Reduce(productTypes []vangogh_integration.ProductType) error {

	if len(productTypes) == 0 {
		productTypes = slices.Collect(vangogh_integration.AllProductTypes())
	}

	for _, pt := range productTypes {
		if pt == vangogh_integration.UnknownProductType {
			continue
		}
		if err := reduceProductType(pt); err != nil {
			return err
		}
	}

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

	catalogAccountProducts, err := shared_data.GetCatalogAccountProducts(-1)
	if err != nil {
		return err
	}

	switch pt {
	case vangogh_integration.UserAccessToken:
		// do nothing
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
	case vangogh_integration.PcgwGogPageId:
		gogIds, err := pcgw_data.GetGameGogIds(catalogAccountProducts)
		if err != nil {
			return err
		}
		return pcgw_data.ReduceGogPageIds(gogIds, kvPt)
	case vangogh_integration.PcgwSteamPageId:
		steamGogIds, err := shared_data.GetSteamGogIds(maps.Keys(catalogAccountProducts))
		if err != nil {
			return err
		}
		return pcgw_data.ReduceSteamPageIds(steamGogIds, kvPt)
	case vangogh_integration.PcgwExternalLinks:
		pcgwGogIds, err := shared_data.GetPcgwGogIds(maps.Keys(catalogAccountProducts))
		if err != nil {
			return err
		}
		return pcgw_data.ReduceExternalLinks(pcgwGogIds, kvPt)
	case vangogh_integration.PcgwEngine:
		pcgwGogIds, err := shared_data.GetPcgwGogIds(maps.Keys(catalogAccountProducts))
		if err != nil {
			return err
		}
		return pcgw_data.ReduceEngine(pcgwGogIds, kvPt)
	case vangogh_integration.HltbRootPage:
		// do nothing
	case vangogh_integration.HltbData:
		return hltb_data.ReduceData(kvPt, -1)
	case vangogh_integration.ProtonDbSummary:
		return protondb_data.ReduceSummary(kvPt, -1)
	default:
		return errors.New("reduction is not supported for " + pt.String())
	}

	return nil
}
