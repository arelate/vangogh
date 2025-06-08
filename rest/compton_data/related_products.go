package compton_data

import "github.com/arelate/southern_light/vangogh_integration"

var OfferingsProperties = []string{
	vangogh_integration.IncludesGamesProperty,
	vangogh_integration.RequiresGamesProperty,
	vangogh_integration.IsIncludedByGamesProperty,
	vangogh_integration.IsRequiredByGamesProperty,
	vangogh_integration.ModifiesGamesProperty,
	vangogh_integration.IsModifiedByGamesProperty,
}
