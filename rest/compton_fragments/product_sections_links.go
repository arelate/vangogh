package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
)

func ProductSectionsLinks(r compton.Registrar, sections []string) compton.Element {

	linksStack := flex_items.FlexItems(r, direction.Row).
		JustifyContent(align.Center).
		FontSize(size.Small).
		FontWeight(font_weight.Bolder).
		RowGap(size.Small)

	for _, s := range sections {
		title := compton_data.SectionTitles[s]
		link := els.A("#" + title)
		linkText := fspan.Text(r, title)
		link.Append(linkText)
		linksStack.Append(link)
	}

	wrapper := flex_items.Center(r, linksStack)
	wrapper.SetId("product-sections-links")

	return wrapper
}
