package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var DigestProperties = []string{
	vangogh_integration.TagIdProperty,
	vangogh_integration.LocalTagsProperty,
	vangogh_integration.SteamDeckAppCompatibilityCategoryProperty,
	vangogh_integration.SteamOsAppCompatibilityCategoryProperty,
	vangogh_integration.OperatingSystemsProperty,
	vangogh_integration.HltbPlatformsProperty,
	vangogh_integration.GenresProperty,
	vangogh_integration.ThemesProperty,
	vangogh_integration.HltbGenresProperty,
	vangogh_integration.StoreTagsProperty,
	vangogh_integration.ProtonDBTierProperty,
	vangogh_integration.ProtonDBConfidenceProperty,
	vangogh_integration.OpenCriticTierProperty,
	vangogh_integration.FeaturesProperty,
	vangogh_integration.GameModesProperty,
	vangogh_integration.LanguageCodeProperty,
	vangogh_integration.ProductTypeProperty,
	vangogh_integration.TypesProperty,
	vangogh_integration.SummaryReviewsProperty,
	vangogh_integration.SteamReviewScoreDescProperty,
	vangogh_integration.TopPercentProperty,
	vangogh_integration.ProductValidationResultProperty,
	vangogh_integration.SortProperty,
}

var BinaryDigestProperties = []string{
	vangogh_integration.UserWishlistProperty,
	vangogh_integration.OwnedProperty,
	vangogh_integration.IsFreeProperty,
	vangogh_integration.IsDiscountedProperty,
	vangogh_integration.PreOrderProperty,
	vangogh_integration.ComingSoonProperty,
	vangogh_integration.InDevelopmentProperty,
	vangogh_integration.DescendingProperty,
	vangogh_integration.IsDemoProperty,
	vangogh_integration.IsModProperty,
}
