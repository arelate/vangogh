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

	s := compton_fragments.ProductSection(compton_data.InformationSection, id, rdx)

	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if rc, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && rc != "" {
			s.SetAttribute("style", "--c-rep:"+rc)
		}
	}

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)
	s.Append(pageStack)

	if shortDesc, yes := rdx.GetLastVal(vangogh_integration.ShortDescriptionProperty, id); yes && strings.TrimSpace(shortDesc) != "" {
		shortDescSpan := compton.Fspan(s, shortDesc).
			MaxWidth(size.MaxWidth).
			ForegroundColor(color.RepGray).
			BorderRadius(size.XSmall).
			FontSize(size.Small).
			TextAlign(align.Start)

		pageStack.Append(
			compton.FICenter(s, shortDescSpan),
			compton.SectionDivider(s, compton.Text("Product Details")))

	}

	items := compton.FlexItems(s, direction.Row).
		JustifyContent(align.Start).
		RowGap(size.Normal).
		ColumnGap(size.Large).
		ColumnWidthRule(size.XXXSmall)

	pageStack.Append(items)

	for _, pp := range compton_fragments.ProductProperties(s, id, rdx, compton_data.ProductProperties...) {
		items.Append(pp)
	}

	return s
}
