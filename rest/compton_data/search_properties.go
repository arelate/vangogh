package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var SearchProperties = []string{

	vangogh_integration.GogTitleProperty,
	vangogh_integration.GogOperatingSystemsProperty,
	vangogh_integration.GogDevelopersProperty,
	vangogh_integration.GogPublishersProperty,

	vangogh_integration.GogIsAccountProductProperty,
	vangogh_integration.GogOwnedProperty,

	vangogh_integration.GogTagIdProperty,
	vangogh_integration.VangoghLocalTagsProperty,
	vangogh_integration.GogUserWishlistProperty,

	vangogh_integration.GogProductTypeProperty,
	vangogh_integration.GogLanguageCodeProperty,

	vangogh_integration.GogIsFreeProperty,
	vangogh_integration.GogIsDemoProperty,
	vangogh_integration.GogIsModProperty,
	vangogh_integration.GogIsDiscountedProperty,
	vangogh_integration.GogPreOrderProperty,
	vangogh_integration.GogComingSoonProperty,
	vangogh_integration.GogInDevelopmentProperty,

	vangogh_integration.GogSeriesProperty,
	vangogh_integration.GogGenresProperty,
	vangogh_integration.GogThemesProperty,
	vangogh_integration.GogStoreTagsProperty,
	vangogh_integration.GogFeaturesProperty,
	vangogh_integration.GogGameModesProperty,

	vangogh_integration.GogGlobalReleaseDateProperty,
	vangogh_integration.GogReleaseDateProperty,
	vangogh_integration.GogOrderDateProperty,

	vangogh_integration.GogProductValidationResultProperty,
	vangogh_integration.GogProductGeneratedChecksumProperty,

	vangogh_integration.UrlSortParameter,
	vangogh_integration.UrlDescendingParameter,
}
