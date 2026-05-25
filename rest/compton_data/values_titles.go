package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var BinaryTitles = map[string]string{
	vangogh_integration.TrueValue:  "Yes",
	vangogh_integration.FalseValue: "No",
}

var TypesTitles = map[string]string{
	vangogh_integration.GogLicences.String():                  "GOG Licences",
	vangogh_integration.GogUserWishlist.String():              "GOG User Wishlist",
	vangogh_integration.GogAccountPage.String():               "GOG Account Page",
	vangogh_integration.GogApiProducts.String():               "GOG API Products",
	vangogh_integration.GogCatalogPage.String():               "GOG Catalog Page",
	vangogh_integration.GogDetails.String():                   "GOG Details",
	vangogh_integration.GogOrderPage.String():                 "GOG Order Page",
	vangogh_integration.HltbData.String():                     "HowLongToBeat Data",
	vangogh_integration.HltbRootPage.String():                 "HowLongToBeat Root Page",
	vangogh_integration.PcgwSteamPageId.String():              "PCGamingWiki Steam PageId",
	vangogh_integration.PcgwGogPageId.String():                "PCGamingWiki GOG PageId",
	vangogh_integration.PcgwRaw.String():                      "PCGamingWiki Raw",
	vangogh_integration.WikipediaRaw.String():                 "Wikipedia Raw",
	vangogh_integration.SteamAppDetails.String():              "Steam App Details",
	vangogh_integration.SteamAppNews.String():                 "Steam App News",
	vangogh_integration.SteamAppReviews.String():              "Steam Reviews",
	vangogh_integration.ProtonDbSummary.String():              "ProtonDB Summary",
	vangogh_integration.GamesDbGogProducts.String():           "GamesDB GOG Products",
	vangogh_integration.SteamDeckCompatibilityReport.String(): "Steam Deck Compat Report",
	vangogh_integration.OpenCriticApiGame.String():            "OpenCritic API Game",
}

var OperatingSystemTitles = map[string]string{
	vangogh_integration.MacOS.String():   "macOS",
	vangogh_integration.Linux.String():   "Linux",
	vangogh_integration.Windows.String(): "Windows",
}
