package compton_data

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
)

var CompatibilitySymbols = map[string]compton.Symbol{
	"Platinum":    compton.CirclePositive,
	"Gold":        compton.CirclePositive,
	"Verified":    compton.CirclePositive,
	"Silver":      compton.TriangleNeutral,
	"Bronze":      compton.TriangleNeutral,
	"Playable":    compton.TriangleNeutral,
	"Borked":      compton.CrossNegative,
	"Unsupported": compton.CrossNegative,
}

var CompatibilityColors = map[string]color.Color{
	"Platinum":    color.Green,
	"Gold":        color.Green,
	"Verified":    color.Green,
	"Silver":      color.Orange,
	"Bronze":      color.Orange,
	"Playable":    color.Orange,
	"Borked":      color.Red,
	"Unsupported": color.Red,
}
