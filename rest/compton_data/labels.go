package compton_data

import (
	"github.com/arelate/vangogh_local_data"
)

var LabelProperties = []string{
	vangogh_local_data.OwnedProperty,
	vangogh_local_data.ProductTypeProperty,
	vangogh_local_data.ComingSoonProperty,
	vangogh_local_data.PreOrderProperty,
	vangogh_local_data.InDevelopmentProperty,
	vangogh_local_data.TagIdProperty,
	vangogh_local_data.LocalTagsProperty,
	vangogh_local_data.IsFreeProperty,
	vangogh_local_data.DiscountPercentageProperty,
	vangogh_local_data.WishlistedProperty,
}

var LabelTitles = map[string]string{
	vangogh_local_data.OwnedProperty:         "Own",
	vangogh_local_data.ComingSoonProperty:    "Soon",
	vangogh_local_data.PreOrderProperty:      "PO",
	vangogh_local_data.InDevelopmentProperty: "In Dev",
	vangogh_local_data.IsFreeProperty:        "Free",
	vangogh_local_data.WishlistedProperty:    "Wish",
}
