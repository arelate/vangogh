package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func Reception(id string, sar *steam_integration.AppReviews, rdx redux.Readable, permissions ...author.Permission) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.ReceptionSection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)

	s.Append(pageStack)

	ratingsRow := compton.FlexItems(s, direction.Row).
		RowGap(size.Normal).
		ColumnGap(size.Large).
		ColumnWidthRule(size.XXXSmall)
	pageStack.Append(ratingsRow)

	for _, rrp := range compton_fragments.ProductProperties(s, id, rdx,
		permittedPropertiesOnly(compton_data.ReceptionProperties, permissions...)) {
		ratingsRow.Append(rrp)
	}

	if sar == nil {
		return s
	}

	pageStack.Append(compton.SectionDivider(s, compton.Text("Steam Reviews")))

	if len(sar.Reviews) == 0 {
		fs := compton.Fspan(s, "Steam reviews are not available for this product").
			ForegroundColor(color.RepGray).
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
