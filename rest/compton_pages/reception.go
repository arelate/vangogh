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
	"github.com/boggydigital/redux"
)

func Reception(id string, sar *steam_integration.AppReviews, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.ReceptionSection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)

	s.Append(pageStack)

	ratingsRow := compton.FlexItems(s, direction.Row).RowGap(size.Normal).ColumnGap(size.Large)
	pageStack.Append(ratingsRow)

	for _, rrp := range compton_fragments.ProductProperties(s, id, rdx, compton_data.ReceptionProperties...) {
		rrp.AddClass("rating")
		ratingsRow.Append(rrp)
	}

	if sar == nil {
		return s
	}

	//steamReviewsRow := compton.FlexItems(s, direction.Row).
	//	AlignItems(align.Center).
	//	JustifyContent(align.Center).
	//	ColumnGap(size.Small).
	//	BackgroundColor(color.Background)
	//steamReviewsRow.AddClass("steam-reviews")
	//steamReviewsRow.Append(
	//	compton.Fspan(s, "Steam Reviews").
	//		FontSize(size.Small).
	//		PaddingBlock(size.Small).
	//		BorderRadius(size.XSmall))
	pageStack.Append(compton.SectionDivider(s, compton.Text("Steam Reviews")))

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
