package compton_data

import "github.com/boggydigital/compton"

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
