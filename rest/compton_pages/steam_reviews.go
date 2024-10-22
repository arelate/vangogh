package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
	"github.com/boggydigital/compton/page"
)

func SteamReviews(id string, sar *steam_integration.AppReviews) *page.PageElement {

	s := compton_fragments.ProductSection(compton_data.SteamReviewsSection)

	pageStack := flex_items.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if len(sar.Reviews) == 0 {
		fs := fspan.Text(s, "Steam reviews are not available for this product").
			ForegroundColor(color.Gray)
		pageStack.Append(flex_items.Center(s, fs))
	}

	for ii, review := range sar.Reviews {
		if srf := compton_fragments.SteamReview(s, review); srf != nil {
			pageStack.Append(srf)
		}
		if ii < len(sar.Reviews)-1 {
			pageStack.Append(els.Hr())
		}
	}

	return s
}
