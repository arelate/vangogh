package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
)

func SteamReviews(id string, sar *steam_integration.AppReviews) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.SteamReviewsSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	summaryRow := compton.Frow(s).FontSize(size.Small)
	summaryRow.PropVal("Overall", sar.GetReviewScoreDesc())

	pageStack.Append(compton.FICenter(s, summaryRow))

	if len(sar.Reviews) == 0 {
		fs := compton.Fspan(s, "Steam reviews are not available for this product").
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(s, fs))
	}

	for ii, review := range sar.Reviews {
		if srf := compton_fragments.SteamReview(s, review); srf != nil {
			pageStack.Append(srf)
		}
		if ii < len(sar.Reviews)-1 {
			pageStack.Append(compton.Hr())
		}
	}

	return s
}
