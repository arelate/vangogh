package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/gog_data"
	"github.com/arelate/vangogh/cli/hltb_data"
	"github.com/arelate/vangogh/cli/protondb_data"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/arelate/vangogh/cli/steam_data"
	"github.com/boggydigital/kevlar"
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
		vangogh_integration.AccountPage,
		vangogh_integration.ApiProducts,
		vangogh_integration.OrderPage,
		vangogh_integration.Details,
		vangogh_integration.GamesDbGogProducts,
		// Steam
		vangogh_integration.SteamAppDetails,
		vangogh_integration.SteamAppNews,
		vangogh_integration.SteamAppReviews,
		vangogh_integration.SteamDeckCompatibilityReport,
		// PCGW - requires special data processing and will be skipped
		// ...
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
