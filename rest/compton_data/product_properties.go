package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var ProductProperties = []string{
	vangogh_integration.TagIdProperty,
	vangogh_integration.LocalTagsProperty,
	vangogh_integration.UserWishlistProperty,

	vangogh_integration.OperatingSystemsProperty,
	vangogh_integration.DevelopersProperty,
	vangogh_integration.PublishersProperty,

	vangogh_integration.CreatorsProperty,
	vangogh_integration.DirectorsProperty,
	vangogh_integration.ProducersProperty,
	vangogh_integration.DesignersProperty,
	vangogh_integration.ProgrammersProperty,
	vangogh_integration.ArtistsProperty,
	vangogh_integration.WritersProperty,
	vangogh_integration.ComposersProperty,

	vangogh_integration.PriceProperty,
	vangogh_integration.LanguageCodeProperty,

	vangogh_integration.SeriesProperty,
	vangogh_integration.GenresProperty,
	vangogh_integration.ThemesProperty,
	vangogh_integration.HltbGenresProperty,
	vangogh_integration.StoreTagsProperty,
	vangogh_integration.FeaturesProperty,
	vangogh_integration.GameModesProperty,

	vangogh_integration.EnginesProperty,
	vangogh_integration.EnginesBuildsProperty,

	vangogh_integration.ProtonDBTierProperty,
	vangogh_integration.ProtonDBConfidenceProperty,

	vangogh_integration.HltbPlatformsProperty,
	vangogh_integration.HltbHoursToCompleteMainProperty,
	vangogh_integration.HltbHoursToCompletePlusProperty,
	vangogh_integration.HltbHoursToComplete100Property,

	vangogh_integration.GlobalReleaseDateProperty,
	vangogh_integration.GOGReleaseDateProperty,
	vangogh_integration.GOGOrderDateProperty,
}

var ProductExternalLinksProperties = []string{
	GOGLinksProperty,
	SteamLinksProperty,
	OtherLinksProperty,
}
