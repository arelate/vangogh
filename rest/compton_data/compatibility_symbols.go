package compton_data

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
)

var CompatibilitySymbols = map[string]compton.Symbol{
	"Platinum":    compton.Circle,
	"Gold":        compton.Circle,
	"Verified":    compton.Circle,
	"Silver":      compton.Triangle,
	"Bronze":      compton.Triangle,
	"Playable":    compton.Triangle,
	"Borked":      compton.Cross,
	"Unsupported": compton.Cross,
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
