package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
)

var ReceptionSymbols = map[string]compton.Symbol{
	vangogh_integration.RatingPositive: compton.ThreeUpwardChevrons,
	vangogh_integration.RatingMixed:    compton.ThreeHorizontalLines,
	vangogh_integration.RatingNegative: compton.ThreeDownwardChevrons,
}

var ReceptionColors = map[string]color.Color{
	vangogh_integration.RatingPositive: color.Green,
	vangogh_integration.RatingMixed:    color.Yellow,
	vangogh_integration.RatingNegative: color.Red,
}
