package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
)

func SteamDeckCompatibility(id string, rdx redux.Readable) (string, color.Color) {
	if sdccp, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {
		//sdc := compton.Fspan(r, sdccp).
		//	FontSize(size.Small).
		//	FontWeight(font_weight.Normal).
		//	PaddingInline(size.XSmall).
		//	PaddingBlock(size.XXSmall).
		//	BorderRadius(size.XXSmall)

		var c color.Color
		switch sdccp {
		case "Verified":
			c = color.Green
		case "Playable":
			c = color.Orange
		case "Unsupported":
			c = color.Red
		default:
			c = color.Gray
		}

		return sdccp, c
	}
	return "", color.Transparent
}
