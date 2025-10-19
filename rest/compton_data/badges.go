package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var InformationBadgeProperties = []string{
	vangogh_integration.OwnedProperty,
	vangogh_integration.UserWishlistProperty,
	vangogh_integration.IsModProperty,
	vangogh_integration.StoreTagsProperty, // Good Old Game label
	vangogh_integration.ProductTypeProperty,
	vangogh_integration.ComingSoonProperty,
	vangogh_integration.PreOrderProperty,
	vangogh_integration.InDevelopmentProperty,
	vangogh_integration.IsFreeProperty,
	vangogh_integration.DiscountPercentageProperty,
	vangogh_integration.TagIdProperty,
	vangogh_integration.LocalTagsProperty,
	vangogh_integration.IsDemoProperty,
	vangogh_integration.DownloadQueuedProperty,
	vangogh_integration.DownloadStartedProperty,
	vangogh_integration.DownloadCompletedProperty,
}
