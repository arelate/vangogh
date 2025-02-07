package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var LabelProperties = []string{
	vangogh_integration.StoreTagsProperty, // Good Old Game label
	//vangogh_integration.OwnedProperty,
	vangogh_integration.ProductTypeProperty,
	vangogh_integration.ComingSoonProperty,
	vangogh_integration.PreOrderProperty,
	vangogh_integration.InDevelopmentProperty,
	vangogh_integration.IsFreeProperty,
	vangogh_integration.DiscountPercentageProperty,
	vangogh_integration.TagIdProperty,
	vangogh_integration.LocalTagsProperty,
	//vangogh_integration.WishlistedProperty,
}

var LabelTitles = map[string]string{
	//vangogh_integration.OwnedProperty:         "Own",
	vangogh_integration.ComingSoonProperty:    "Soon",
	vangogh_integration.PreOrderProperty:      "PO",
	vangogh_integration.InDevelopmentProperty: "In Dev",
	vangogh_integration.IsFreeProperty:        "Free",
	//vangogh_integration.WishlistedProperty:    "Wish",
}
