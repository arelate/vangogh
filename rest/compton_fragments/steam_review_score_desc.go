package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
)

func SteamReviewScoreDesc(id string, rdx redux.Readable) (string, color.Color) {

	if srsdp, ok := rdx.GetLastVal(vangogh_integration.SteamReviewScoreDescProperty, id); ok {

		//srsd := compton.Fspan(r, srsdp).
		//	FontSize(size.Small).
		//	FontWeight(font_weight.Normal).
		//	PaddingInline(size.XSmall).
		//	PaddingBlock(size.XXSmall).
		//	BorderRadius(size.XXSmall)

		var c color.Color
		switch reviewClass(srsdp) {
		case "positive":
			c = color.Green
		case "negative":
			c = color.Red
		default:
			c = color.Gray
		}

		return srsdp, c

	}

	return "", color.Transparent
}
