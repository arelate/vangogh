package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
)

func SteamReviews(id string, sar *steam_integration.AppReviews) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.SteamReviewsSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if len(sar.Reviews) == 0 {
		fs := compton.Fspan(s, "Steam reviews are not available for this product").
			ForegroundColor(color.Gray)
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
