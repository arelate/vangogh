package compton_data

import "github.com/arelate/vangogh_local_data"

var DigestProperties = []string{
	vangogh_local_data.TagIdProperty,
	//vangogh_local_data.LocalTagsProperty,
	vangogh_local_data.SteamDeckAppCompatibilityCategoryProperty,
	vangogh_local_data.OperatingSystemsProperty,
	vangogh_local_data.LanguageCodeProperty,
	vangogh_local_data.ProductTypeProperty,
	vangogh_local_data.TypesProperty,
	//vangogh_local_data.SteamReviewScoreDescProperty,
	//vangogh_local_data.ValidationResultProperty,
	vangogh_local_data.SortProperty,
}

var BinaryDigestProperties = []string{
	vangogh_local_data.WishlistedProperty,
	vangogh_local_data.OwnedProperty,
	vangogh_local_data.IsFreeProperty,
	vangogh_local_data.IsDiscountedProperty,
	vangogh_local_data.PreOrderProperty,
	vangogh_local_data.ComingSoonProperty,
	vangogh_local_data.InDevelopmentProperty,
	vangogh_local_data.DescendingProperty,
}
