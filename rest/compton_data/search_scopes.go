package compton_data

import (
	"net/url"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
)

const (
	SearchNew      = "New"
	SearchGog      = "GOG"
	SearchWishlist = "Wish"
	SearchSale     = "Sale"
)

var SearchScopesSymbols = map[string]compton.Symbol{
	SearchNew:      compton.TwoTitleValues,
	SearchGog:      compton.LetterG,
	SearchWishlist: compton.Heart,
	SearchSale:     compton.Percent,
}

const GogPreservationProgramTag = "Good Old Game"

var SearchOrder = []string{
	SearchNew,
	SearchGog,
	SearchWishlist,
	SearchSale,
}

func SearchScopes() map[string]string {

	queries := make(map[string]string)

	queries[SearchNew] = ""

	q := make(url.Values)
	q.Set(vangogh_integration.OwnedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.ProductTypeProperty, vangogh_integration.GameProductType)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGOrderDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchGog] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.UserWishlistProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GOGReleaseDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchWishlist] = q.Encode()

	q = make(url.Values)
	q.Set(vangogh_integration.TypesProperty, vangogh_integration.GogApiProducts.String())
	q.Set(vangogh_integration.OwnedProperty, vangogh_integration.FalseValue)
	q.Set(vangogh_integration.IsDiscountedProperty, vangogh_integration.TrueValue)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.DiscountPercentageProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	queries[SearchSale] = q.Encode()

	//q = make(url.Values)
	//q.Set(vangogh_integration.IsModProperty, vangogh_integration.TrueValue)
	//queries[SearchMods] = q.Encode()
	//
	//q = make(url.Values)
	//q.Set(vangogh_integration.StoreTagsProperty, GogPreservationProgramTag)
	//queries[SearchGogPreservationProgram] = q.Encode()

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

	var searchScope string
	if len(query) == 0 {
		searchScope = ""
	}

	for st, sq := range SearchScopes() {
		if sq == enq {
			searchScope = st
		}
	}

	return searchScope
}
