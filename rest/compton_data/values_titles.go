package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var BinaryTitles = map[string]string{
	vangogh_integration.TrueValue:  "Yes",
	vangogh_integration.FalseValue: "No",
}

var TypesTitles = map[string]string{
	vangogh_integration.AccountProducts.String():              "Account Products",
	vangogh_integration.ApiProducts.String():                  "API Products",
	vangogh_integration.CatalogProducts.String():              "Catalog Products",
	vangogh_integration.Details.String():                      "Details",
	vangogh_integration.HltbData.String():                     "HowLongToBeat Data",
	vangogh_integration.HltbRootPage.String():                 "HowLongToBeat Root Page",
	vangogh_integration.Orders.String():                       "Orders",
	vangogh_integration.PcgwEngine.String():                   "PCGamingWiki Engine",
	vangogh_integration.PcgwExternalLinks.String():            "PCGamingWiki External Links",
	vangogh_integration.PcgwSteamPageId.String():              "PCGamingWiki Steam PageId",
	vangogh_integration.PcgwGogPageId.String():                "PCGamingWiki GOG PageId",
	vangogh_integration.SteamAppNews.String():                 "Steam App News",
	vangogh_integration.SteamAppReviews.String():              "Steam Reviews",
	vangogh_integration.ProtonDbSummary.String():              "ProtonDB Summary",
	vangogh_integration.UserWishlistProducts.String():         "User Wishlist Products",
	vangogh_integration.GamesDbGogProducts.String():           "GamesDB GOG Products",
	vangogh_integration.SteamDeckCompatibilityReport.String(): "Steam Deck Compat Report",
}

var OperatingSystemTitles = map[string]string{
	vangogh_integration.MacOS.String():   "macOS",
	vangogh_integration.Linux.String():   "Linux",
	vangogh_integration.Windows.String(): "Windows",
}
