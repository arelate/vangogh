package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var InformationBadgeProperties = []string{
	// symbols
	vangogh_integration.GogOwnedProperty,
	vangogh_integration.DownloadQueuedProperty,
	vangogh_integration.DownloadStartedProperty,
	//vangogh_integration.DownloadCompletedProperty,
	vangogh_integration.GogProductValidationResultProperty,
	vangogh_integration.GogUserWishlistProperty,
	vangogh_integration.GogIsModProperty,
	vangogh_integration.GogStoreTagsProperty, // Good Old Game label
	vangogh_integration.GogProductTypeProperty,
	vangogh_integration.TopPercentProperty,
	// text
	vangogh_integration.GogComingSoonProperty,
	vangogh_integration.GogPreOrderProperty,
	vangogh_integration.GogInDevelopmentProperty,
	vangogh_integration.GogIsFreeProperty,
	vangogh_integration.GogDiscountPercentageProperty,
	vangogh_integration.GogTagIdProperty,
	vangogh_integration.LocalTagsProperty,
	vangogh_integration.GogIsDemoProperty,
}
