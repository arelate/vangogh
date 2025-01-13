package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var DigestProperties = []string{
	vangogh_integration.TagIdProperty,
	vangogh_integration.LocalTagsProperty,
	vangogh_integration.SteamDeckAppCompatibilityCategoryProperty,
	vangogh_integration.OperatingSystemsProperty,
	vangogh_integration.LanguageCodeProperty,
	vangogh_integration.ProductTypeProperty,
	vangogh_integration.TypesProperty,
	vangogh_integration.SteamReviewScoreDescProperty,
	vangogh_integration.ProductValidationResultProperty,
	vangogh_integration.SortProperty,
}

var BinaryDigestProperties = []string{
	vangogh_integration.WishlistedProperty,
	vangogh_integration.OwnedProperty,
	vangogh_integration.IsFreeProperty,
	vangogh_integration.IsDiscountedProperty,
	vangogh_integration.PreOrderProperty,
	vangogh_integration.ComingSoonProperty,
	vangogh_integration.InDevelopmentProperty,
	vangogh_integration.DescendingProperty,
}
