package compton_data

import (
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
)

const (
	SearchResultsLimit = 60 // divisible by 2,3,4,5,6
)

const (
	GogSectionSearchUrl   = "/gog/search"
	GogSectionOwnedUrl    = "/gog/owned"
	GogSectionCatalogUrl  = "/gog/catalog"
	GogSectionWishlistUrl = "/gog/wishlist"
	GogSectionSaleUrl     = "/gog/sale"
)

func AllGogSectionUrls() []string {
	return []string{
		GogSectionSearchUrl,
		GogSectionOwnedUrl,
		GogSectionCatalogUrl,
		GogSectionWishlistUrl,
		GogSectionSaleUrl,
	}
}

var GogSectionTitles = map[string]string{
	GogSectionSearchUrl:   "Search",
	GogSectionOwnedUrl:    "Owned",
	GogSectionCatalogUrl:  "Catalog",
	GogSectionWishlistUrl: "Wishlist",
	GogSectionSaleUrl:     "Sale",
}

var GogSectionSymbols = map[string]compton.Symbol{
	GogSectionSearchUrl:   compton.Search,
	GogSectionOwnedUrl:    compton.CircleCompactDisk,
	GogSectionCatalogUrl:  compton.ShoppingLabel,
	GogSectionWishlistUrl: compton.Heart,
	GogSectionSaleUrl:     compton.Percent,
}

func GogSectionSearchQuery(sectionUrl string) url.Values {
	q := make(url.Values)

	switch sectionUrl {
	case GogSectionOwnedUrl:
		q.Set(vangogh_integration.GogIsAccountProductProperty, vangogh_integration.TrueValue)
	case GogSectionSaleUrl:
		q.Set(vangogh_integration.GogOwnedProperty, vangogh_integration.FalseValue)
		q.Set(vangogh_integration.GogIsDiscountedProperty, vangogh_integration.TrueValue)
	case GogSectionWishlistUrl:
		q.Set(vangogh_integration.GogUserWishlistProperty, vangogh_integration.TrueValue)
	case GogSectionCatalogUrl:
	}

	return q
}

const (
	SortByPurchaseDate         = "purchases"
	SortByDownloadUpdated      = "downloads"
	SortByGogReleaseDate       = "releases"
	SortBySteamCommunityUpdate = "news"
	SortByDiscount             = "discount"
	SortByGogRating            = "ratings"
)

var GogSectionSortBy = map[string][]string{
	GogSectionOwnedUrl:    {SortByPurchaseDate, SortByDownloadUpdated},
	GogSectionCatalogUrl:  {SortByGogReleaseDate, SortBySteamCommunityUpdate},
	GogSectionWishlistUrl: {SortByGogReleaseDate, SortByDiscount},
	GogSectionSaleUrl:     {SortByDiscount, SortByGogRating},
}

var SortByParameters = map[string]map[string]string{
	SortByPurchaseDate: {
		vangogh_integration.UrlSortParameter:       vangogh_integration.GogAccountProductOrderProperty,
		vangogh_integration.UrlDescendingParameter: vangogh_integration.FalseValue,
	},
	SortByDownloadUpdated: {
		vangogh_integration.UrlSortParameter:       vangogh_integration.VangoghDownloadCompletedProperty,
		vangogh_integration.UrlDescendingParameter: vangogh_integration.TrueValue,
	},
	SortByGogReleaseDate: {
		vangogh_integration.UrlSortParameter:       vangogh_integration.GogReleaseDateProperty,
		vangogh_integration.UrlDescendingParameter: vangogh_integration.TrueValue,
	},
	SortBySteamCommunityUpdate: {
		vangogh_integration.UrlSortParameter:       vangogh_integration.VangoghSteamLastCommunityUpdateProperty,
		vangogh_integration.UrlDescendingParameter: vangogh_integration.TrueValue,
	},
	SortByDiscount: {
		vangogh_integration.GogIsDiscountedProperty: vangogh_integration.TrueValue,
		vangogh_integration.UrlSortParameter:        vangogh_integration.GogDiscountPercentageProperty,
		vangogh_integration.UrlDescendingParameter:  vangogh_integration.TrueValue,
	},
	SortByGogRating: {
		vangogh_integration.UrlSortParameter:       vangogh_integration.GogRatingProperty,
		vangogh_integration.UrlDescendingParameter: vangogh_integration.TrueValue,
	},
}

var SortByTitles = map[string]string{
	SortByPurchaseDate:         "Purchase",
	SortByDownloadUpdated:      "Updated",
	SortByGogReleaseDate:       "Release",
	SortBySteamCommunityUpdate: "News",
	SortByDiscount:             "Discount",
	SortByGogRating:            "Rating",
}
