package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var ProductProperties = []string{
	vangogh_integration.TagIdProperty,
	vangogh_integration.LocalTagsProperty,

	vangogh_integration.UserWishlistProperty,

	vangogh_integration.PriceProperty,
	vangogh_integration.OperatingSystemsProperty,
	vangogh_integration.HltbPlatformsProperty,
	vangogh_integration.RatingProperty,
	vangogh_integration.SteamReviewScoreDescProperty,
	vangogh_integration.HltbReviewScoreProperty,
	vangogh_integration.AggregatedRatingProperty,
	vangogh_integration.ProtonDBTierProperty,
	vangogh_integration.ProtonDBConfidenceProperty,
	vangogh_integration.DevelopersProperty,
	vangogh_integration.PublishersProperty,
	vangogh_integration.EnginesProperty,
	vangogh_integration.EnginesBuildsProperty,
	vangogh_integration.SeriesProperty,
	vangogh_integration.GenresProperty,
	vangogh_integration.ThemesProperty,
	vangogh_integration.HltbGenresProperty,
	vangogh_integration.StoreTagsProperty,
	vangogh_integration.FeaturesProperty,
	vangogh_integration.GameModesProperty,
	vangogh_integration.LanguageCodeProperty,
	vangogh_integration.GlobalReleaseDateProperty,
	vangogh_integration.GOGReleaseDateProperty,
	vangogh_integration.GOGOrderDateProperty,

	vangogh_integration.HltbHoursToCompleteMainProperty,
	vangogh_integration.HltbHoursToCompletePlusProperty,
	vangogh_integration.HltbHoursToComplete100Property,
}

var ProductExternalLinksProperties = []string{
	GauginGOGLinksProperty,
	GauginSteamLinksProperty,
	GauginOtherLinksProperty,
}
