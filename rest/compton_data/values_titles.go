package compton_data

import "github.com/arelate/vangogh_local_data"

var BinaryTitles = map[string]string{
	vangogh_local_data.TrueValue:  "Yes",
	vangogh_local_data.FalseValue: "No",
}

var TypesTitles = map[string]string{
	vangogh_local_data.AccountProducts.String():      "Account Products",
	vangogh_local_data.ApiProductsV1.String():        "API Products v1",
	vangogh_local_data.ApiProductsV2.String():        "API Products v2",
	vangogh_local_data.CatalogProducts.String():      "Catalog Products",
	vangogh_local_data.Details.String():              "Details",
	vangogh_local_data.HLTBData.String():             "HowLongToBeat Data",
	vangogh_local_data.HLTBRootPage.String():         "HowLongToBeat Root Page",
	vangogh_local_data.LicenceProducts.String():      "Licence Products",
	vangogh_local_data.Orders.String():               "Orders",
	vangogh_local_data.PCGWEngine.String():           "PCGamingWiki Engine",
	vangogh_local_data.PCGWExternalLinks.String():    "PCGamingWiki External Links",
	vangogh_local_data.PCGWPageId.String():           "PCGamingWiki PageId",
	vangogh_local_data.SteamAppNews.String():         "Steam App News",
	vangogh_local_data.SteamReviews.String():         "Steam Reviews",
	vangogh_local_data.SteamStorePage.String():       "Steam Store Page",
	vangogh_local_data.ProtonDBSummary.String():      "ProtonDB Summary",
	vangogh_local_data.UserWishlistProducts.String(): "User Wishlist Products",
}

var OperatingSystemTitles = map[string]string{
	vangogh_local_data.MacOS.String():   "macOS",
	vangogh_local_data.Linux.String():   "Linux",
	vangogh_local_data.Windows.String(): "Windows",
}
