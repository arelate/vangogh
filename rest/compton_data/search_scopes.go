package compton_data

import (
	"github.com/arelate/vangogh_local_data"
	"net/url"
	"strings"
)

const (
	SearchNew      = "New"
	SearchOwned    = "Owned"
	SearchWishlist = "Wishlist"
	SearchSale     = "Sale"
	SearchAll      = "All"
)

var SearchOrder = []string{
	SearchNew,
	SearchOwned,
	SearchWishlist,
	SearchSale,
	SearchAll,
}

func SearchScopes() map[string]string {

	queries := make(map[string]string)

	queries[SearchNew] = ""

	q := make(url.Values)
	q.Set(vangogh_local_data.TypesProperty, vangogh_local_data.AccountProducts.String())
	q.Set(vangogh_local_data.SortProperty, vangogh_local_data.GOGOrderDateProperty)
	q.Set(vangogh_local_data.DescendingProperty, vangogh_local_data.TrueValue)
	queries[SearchOwned] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_local_data.WishlistedProperty, vangogh_local_data.TrueValue)
	q.Set(vangogh_local_data.SortProperty, vangogh_local_data.GOGReleaseDateProperty)
	q.Set(vangogh_local_data.DescendingProperty, vangogh_local_data.TrueValue)
	queries[SearchWishlist] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_local_data.TypesProperty, vangogh_local_data.CatalogProducts.String())
	q.Set(vangogh_local_data.OwnedProperty, vangogh_local_data.FalseValue)
	q.Set(vangogh_local_data.IsDiscountedProperty, vangogh_local_data.TrueValue)
	q.Set(vangogh_local_data.SortProperty, vangogh_local_data.DiscountPercentageProperty)
	q.Set(vangogh_local_data.DescendingProperty, vangogh_local_data.TrueValue)
	queries[SearchSale] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_local_data.TypesProperty, vangogh_local_data.CatalogProducts.String())
	q.Set(vangogh_local_data.SortProperty, vangogh_local_data.GOGReleaseDateProperty)
	q.Set(vangogh_local_data.DescendingProperty, vangogh_local_data.TrueValue)
	queries[SearchAll] = q.Encode()

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
