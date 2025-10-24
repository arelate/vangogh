package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func Offerings(id string, rdx redux.Readable, permissions ...author.Permission) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.OfferingsSection, id, rdx)

	s.AppendSpeculationRules(compton.SpeculationRulesConservativeEagerness, "/*")

	stack := compton.FlexItems(s, direction.Column).RowGap(size.Normal).Width(size.FullWidth)
	s.Append(stack)

	for _, op := range compton_data.OfferingsProperties {
		if rps, ok := rdx.GetAllValues(op, id); ok && len(rps) > 0 {

			ptRow := compton.FlexItems(s, direction.Row).
				AlignItems(align.Center).
				JustifyContent(align.Center).
				ColumnGap(size.Small)

			ptRow.Append(compton.SvgUse(s, compton_data.PropertySymbols[op]), compton.Text(compton_data.PropertyTitles[op]))

			propertyTitleRow := compton.SectionDivider(s, ptRow)

			stack.Append(compton.FICenter(s, propertyTitleRow))
			stack.Append(compton_fragments.ProductsList(s, rps, 0, len(rps), rdx, true, permissions...))
		}
	}

	return s
}
