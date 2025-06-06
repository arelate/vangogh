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
	"strings"
)

func Information(id string, rdx redux.Readable) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.InformationSection)

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)
	s.Append(pageStack)

	if shortDesc, yes := rdx.GetLastVal(vangogh_integration.ShortDescriptionProperty, id); yes && strings.TrimSpace(shortDesc) != "" {
		shortDescSpan := compton.Fspan(s, shortDesc).
			MaxWidth(size.MaxWidth).
			ForegroundColor(color.Gray).
			BorderRadius(size.XSmall).
			FontSize(size.Small).
			TextAlign(align.Start)

		pageStack.Append(
			compton.FICenter(s, shortDescSpan),
			compton.SectionDivider(s, compton.Text("Product Details")))

	}

	grid := compton.GridItems(s).JustifyContent(align.Center).RowGap(size.Normal)
	pageStack.Append(grid)

	for _, pp := range compton_fragments.ProductProperties(s, id, rdx, compton_data.ProductProperties...) {
		grid.Append(pp)
	}

	return s
}
