package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
)

func SteamReviewScoreDesc(r compton.Registrar, id string, rdx kevlar.ReadableRedux) compton.Element {

	if srsdp, ok := rdx.GetLastVal(vangogh_integration.SteamReviewScoreDescProperty, id); ok {

		srsd := compton.Fspan(r, srsdp).
			FontSize(size.Small).
			FontWeight(font_weight.Normal).
			PaddingInline(size.XSmall).
			PaddingBlock(size.XXSmall).
			BorderRadius(size.XXSmall)

		var c color.Color
		switch reviewClass(srsdp) {
		case "positive":
			c = color.Green
		case "negative":
			c = color.Red
		default:
			c = color.Gray
		}

		return srsd.BackgroundColor(c).ForegroundColor(color.Highlight)

	}

	return nil
}
