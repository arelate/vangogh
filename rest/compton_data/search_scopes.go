package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"net/url"
	"strings"
)

const (
	SearchNew                    = "New"
	SearchOwned                  = "Own"
	SearchWishlist               = "Wish"
	SearchSale                   = "Sale"
	SearchGogPreservationProgram = "GPP"
	SearchMods                   = "Mods"
)

const GogPreservationProgramTag = "Good Old Game"

var SearchOrder = []string{
	SearchNew,
	SearchOwned,
	SearchWishlist,
	SearchSale,
	SearchGogPreservationProgram,
	SearchMods,
}

func SearchScopes() map[string]string {

	queries := make(map[string]string)

	queries[SearchNew] = ""

	q := make(url.Values)
	q.Set(vangogh_integration.OwnedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.ProductTypeProperty, vangogh_integration.GameProductType)
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
	q.Set(vangogh_integration.IsModProperty, vangogh_integration.TrueValue)
	queries[SearchMods] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.StoreTagsProperty, GogPreservationProgramTag)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.TitleProperty)
	queries[SearchGogPreservationProgram] = q.Encode()

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
