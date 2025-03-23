package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func RelatedProducts(id string, rdx redux.Readable) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.RelatedProductsSection)
	s.RegisterStyles(compton_styles.Styles, "product-card.css")
	s.RegisterStyles(compton_styles.Styles, "product-labels.css")
	s.AppendSpeculationRules("/product?id=*")

	stack := compton.FlexItems(s, direction.Column)
	s.Append(stack)

	for _, rpp := range compton_data.RelatedProductsProperties {
		if rps, ok := rdx.GetAllValues(rpp, id); ok && len(rps) > 0 {
			propertyTitleRow := compton.FlexItems(s, direction.Row).
				AlignItems(align.Center).
				JustifyContent(align.Center).
				ColumnGap(size.Small).
				BackgroundColor(color.Background)
			propertyTitleRow.AddClass("property-title")
			propertyTitleRow.Append(
				compton.Fspan(s, compton_data.PropertyTitles[rpp]).FontSize(size.Small))

			stack.Append(compton.FICenter(s, propertyTitleRow))
			stack.Append(compton_fragments.ProductsList(s, rps, 0, len(rps), rdx, true))
		}
	}

	return s
}
