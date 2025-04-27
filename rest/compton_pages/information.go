package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func Information(id string, rdx redux.Readable) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.InformationSection)

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Large)
	s.Append(pageStack)

	if shortDesc, yes := rdx.GetLastVal(vangogh_integration.ShortDescriptionProperty, id); yes {
		shortDescSpan := compton.Fspan(s, shortDesc).
			BackgroundColor(color.Highlight).
			ForegroundColor(color.Foreground).
			BorderRadius(size.XSmall).
			PaddingInline(size.Normal).
			PaddingBlock(size.Small).
			FontSize(size.Small).
			TextAlign(align.Start)
		shortDescSpan.AddClass("short-description")

		pageStack.Append(compton.FICenter(s, shortDescSpan))
	}

	grid := compton.GridItems(s).JustifyContent(align.Center).RowGap(size.Normal)
	pageStack.Append(grid)

	for _, pp := range compton_fragments.ProductProperties(s, id, rdx, compton_data.ProductProperties...) {
		grid.Append(pp)
	}

	return s
}
