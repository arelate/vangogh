package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var BinaryTitles = map[string]string{
	vangogh_integration.TrueValue:  "Yes",
	vangogh_integration.FalseValue: "No",
}

var TypesTitles = map[string]string{
	vangogh_integration.AccountProducts.String():              "Account Products",
	vangogh_integration.ApiProductsV1.String():                "API Products v1",
	vangogh_integration.ApiProductsV2.String():                "API Products v2",
	vangogh_integration.CatalogProducts.String():              "Catalog Products",
	vangogh_integration.Details.String():                      "Details",
	vangogh_integration.HLTBData.String():                     "HowLongToBeat Data",
	vangogh_integration.HLTBRootPage.String():                 "HowLongToBeat Root Page",
	vangogh_integration.LicenceProducts.String():              "Licence Products",
	vangogh_integration.Orders.String():                       "Orders",
	vangogh_integration.PCGWEngine.String():                   "PCGamingWiki Engine",
	vangogh_integration.PCGWExternalLinks.String():            "PCGamingWiki External Links",
	vangogh_integration.PCGWPageId.String():                   "PCGamingWiki PageId",
	vangogh_integration.SteamAppNews.String():                 "Steam App News",
	vangogh_integration.SteamReviews.String():                 "Steam Reviews",
	vangogh_integration.SteamStorePage.String():               "Steam Store Page",
	vangogh_integration.ProtonDBSummary.String():              "ProtonDB Summary",
	vangogh_integration.UserWishlistProducts.String():         "User Wishlist Products",
	vangogh_integration.GamesDbProducts.String():              "Galaxy GamesDB Products",
	vangogh_integration.SteamDeckCompatibilityReport.String(): "Steam Deck Compat Report",
}

var OperatingSystemTitles = map[string]string{
	vangogh_integration.MacOS.String():   "macOS",
	vangogh_integration.Linux.String():   "Linux",
	vangogh_integration.Windows.String(): "Windows",
}
