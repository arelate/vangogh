package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func Offerings(id string, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.OfferingsSection)

	s.AppendSpeculationRules("/product?id=*")

	s.RegisterStyles(compton_styles.Styles, "good-old-game-badge.css")

	stack := compton.FlexItems(s, direction.Column).RowGap(size.Normal).Width(size.FullWidth)
	s.Append(stack)

	for _, op := range compton_data.OfferingsProperties {
		if rps, ok := rdx.GetAllValues(op, id); ok && len(rps) > 0 {

			propertyTitleRow := compton.SectionDivider(s, compton.Text(compton_data.PropertyTitles[op]))

			stack.Append(compton.FICenter(s, propertyTitleRow))
			stack.Append(compton_fragments.ProductsList(s, rps, 0, len(rps), rdx, true))
		}
	}

	return s
}
