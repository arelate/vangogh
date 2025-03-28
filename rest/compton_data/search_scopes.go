package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"net/url"
	"strings"
)

const (
	SearchNew      = "New"
	SearchOwned    = "Own"
	SearchWishlist = "Wish"
	SearchSale     = "Sale"
	SearchGOG      = "GOG"
	SearchAll      = "All"
)

var SearchOrder = []string{
	SearchNew,
	SearchOwned,
	SearchWishlist,
	SearchSale,
	SearchGOG,
	SearchAll,
}

func SearchScopes() map[string]string {

	queries := make(map[string]string)

	queries[SearchNew] = ""

	q := make(url.Values)
	q.Set(vangogh_integration.OwnedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.ProductTypeProperty, "GAME")
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGOrderDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchOwned] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.UserWishlistProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGReleaseDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchWishlist] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.TypesProperty, vangogh_integration.ApiProducts.String())
	q.Set(vangogh_integration.OwnedProperty, vangogh_integration.FalseValue)
	q.Set(vangogh_integration.IsDiscountedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.DiscountPercentageProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchSale] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.TypesProperty, vangogh_integration.ApiProducts.String())
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGReleaseDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchAll] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.StoreTagsProperty, "Good Old Game")
	q.Set(vangogh_integration.SortProperty, vangogh_integration.TitleProperty)
	queries[SearchGOG] = q.Encode()

	return queries
}

func EncodeQuery(query map[string][]string) string {
	q := &url.Values{}
	for property, values := range query {
		q.Set(property, strings.Join(values, ", "))
	}

	return q.Encode()
}

func SearchScopeFromQuery(query map[string][]string) string {
	enq := EncodeQuery(query)

	searchScope := SearchNew
	for st, sq := range SearchScopes() {
		if sq == enq {
			searchScope = st
		}
	}

	return searchScope
}
