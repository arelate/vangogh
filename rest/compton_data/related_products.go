package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var OfferingsProperties = []string{
	vangogh_integration.GogIncludesGamesProperty,
	vangogh_integration.GogRequiresGamesProperty,
	vangogh_integration.GogIsIncludedByGamesProperty,
	vangogh_integration.GogIsRequiredByGamesProperty,
	vangogh_integration.GogModifiesGamesProperty,
	vangogh_integration.GogIsModifiedByGamesProperty,
}
