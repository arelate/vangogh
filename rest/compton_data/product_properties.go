package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

var ProductProperties = []string{
	vangogh_integration.GogPriceProperty,

	vangogh_integration.GogTagIdProperty,
	vangogh_integration.VangoghLocalTagsProperty,
	vangogh_integration.GogUserWishlistProperty,

	vangogh_integration.WikipediaCreatorsProperty,
	vangogh_integration.WikipediaDirectorsProperty,
	vangogh_integration.WikipediaProducersProperty,
	vangogh_integration.WikipediaDesignersProperty,
	vangogh_integration.WikipediaProgrammersProperty,
	vangogh_integration.WikipediaArtistsProperty,
	vangogh_integration.WikipediaWritersProperty,
	vangogh_integration.WikipediaComposersProperty,

	vangogh_integration.GogLanguageCodeProperty,

	vangogh_integration.GogSeriesProperty,
	vangogh_integration.GogGenresProperty,
	vangogh_integration.GogThemesProperty,
	vangogh_integration.HltbGenresProperty,
	vangogh_integration.GogStoreTagsProperty,
	vangogh_integration.GogFeaturesProperty,
	vangogh_integration.SteamCategoriesProperty,
	vangogh_integration.GogGameModesProperty,

	vangogh_integration.PcgwEnginesProperty,
	vangogh_integration.PcgwEnginesBuildsProperty,

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
