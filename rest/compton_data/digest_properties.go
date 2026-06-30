package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var DigestProperties = []string{
	vangogh_integration.GogTagIdProperty,
	vangogh_integration.VangoghLocalTagsProperty,
	vangogh_integration.GogOperatingSystemsProperty,
	vangogh_integration.GogGenresProperty,
	vangogh_integration.GogThemesProperty,
	vangogh_integration.GogStoreTagsProperty,
	vangogh_integration.GogFeaturesProperty,
	vangogh_integration.GogGameModesProperty,
	vangogh_integration.GogLanguageCodeProperty,
	vangogh_integration.GogProductTypeProperty,
	vangogh_integration.GogProductValidationResultProperty,
	vangogh_integration.GogProductGeneratedChecksumProperty,
	vangogh_integration.UrlSortParameter,
}

var BinaryDigestProperties = []string{
	vangogh_integration.GogUserWishlistProperty,
	vangogh_integration.GogOwnedProperty,
	vangogh_integration.GogIsFreeProperty,
	vangogh_integration.GogIsDiscountedProperty,
	vangogh_integration.GogPreOrderProperty,
	vangogh_integration.GogComingSoonProperty,
	vangogh_integration.GogInDevelopmentProperty,
	vangogh_integration.UrlDescendingParameter,
	vangogh_integration.GogIsDemoProperty,
	vangogh_integration.GogIsModProperty,
	vangogh_integration.GogProductGeneratedChecksumProperty,
	vangogh_integration.GogIsAccountProductProperty,
}
