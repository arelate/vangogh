package compton_data

import (
	"github.com/arelate/vangogh_local_data"
)

var ProductProperties = []string{
	vangogh_local_data.TagIdProperty,
	vangogh_local_data.LocalTagsProperty,

	vangogh_local_data.WishlistedProperty,
	vangogh_local_data.PriceProperty,
	vangogh_local_data.OperatingSystemsProperty,
	vangogh_local_data.HLTBPlatformsProperty,
	vangogh_local_data.RatingProperty,
	vangogh_local_data.SteamReviewScoreDescProperty,
	vangogh_local_data.HLTBReviewScoreProperty,
	vangogh_local_data.SteamDeckAppCompatibilityCategoryProperty,
	vangogh_local_data.ProtonDBTierProperty,
	vangogh_local_data.ProtonDBConfidenceProperty,
	vangogh_local_data.DevelopersProperty,
	vangogh_local_data.PublishersProperty,
	vangogh_local_data.EnginesProperty,
	vangogh_local_data.EnginesBuildsProperty,
	vangogh_local_data.SeriesProperty,
	vangogh_local_data.GenresProperty,
	vangogh_local_data.HLTBGenresProperty,
	vangogh_local_data.StoreTagsProperty,
	vangogh_local_data.SteamTagsProperty,
	vangogh_local_data.FeaturesProperty,
	vangogh_local_data.LanguageCodeProperty,
	vangogh_local_data.GlobalReleaseDateProperty,
	vangogh_local_data.GOGReleaseDateProperty,
	vangogh_local_data.GOGOrderDateProperty,
	vangogh_local_data.IncludesGamesProperty,
	vangogh_local_data.IsIncludedByGamesProperty,
	vangogh_local_data.RequiresGamesProperty,
	vangogh_local_data.IsRequiredByGamesProperty,

	vangogh_local_data.HLTBHoursToCompleteMainProperty,
	vangogh_local_data.HLTBHoursToCompletePlusProperty,
	vangogh_local_data.HLTBHoursToComplete100Property,
}

var ProductExternalLinksProperties = []string{
	GauginGOGLinksProperty,
	GauginSteamLinksProperty,
	GauginOtherLinksProperty,
}
