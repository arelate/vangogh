package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var ProductProperties = []string{
	vangogh_integration.GogPriceProperty,

	vangogh_integration.GogTagIdProperty,
	vangogh_integration.LocalTagsProperty,
	vangogh_integration.GogUserWishlistProperty,

	vangogh_integration.CreatorsProperty,
	vangogh_integration.DirectorsProperty,
	vangogh_integration.ProducersProperty,
	vangogh_integration.DesignersProperty,
	vangogh_integration.ProgrammersProperty,
	vangogh_integration.ArtistsProperty,
	vangogh_integration.WritersProperty,
	vangogh_integration.ComposersProperty,

	vangogh_integration.LanguageCodeProperty,

	vangogh_integration.GogSeriesProperty,
	vangogh_integration.GogGenresProperty,
	vangogh_integration.GogThemesProperty,
	vangogh_integration.HltbGenresProperty,
	vangogh_integration.GogStoreTagsProperty,
	vangogh_integration.GogFeaturesProperty,
	vangogh_integration.SteamCategoriesProperty,
	vangogh_integration.GogGameModesProperty,

	vangogh_integration.EnginesProperty,
	vangogh_integration.EnginesBuildsProperty,

	vangogh_integration.HltbPlatformsProperty,
	vangogh_integration.HltbHoursToCompleteMainProperty,
	vangogh_integration.HltbHoursToCompletePlusProperty,
	vangogh_integration.HltbHoursToComplete100Property,

	vangogh_integration.GogGlobalReleaseDateProperty,
	vangogh_integration.GogReleaseDateProperty,
	vangogh_integration.GogOrderDateProperty,
}

var ProductExternalLinksProperties = []string{
	GOGLinksProperty,
	SteamLinksProperty,
	OtherLinksProperty,
}
