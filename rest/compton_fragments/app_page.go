package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
)

func AppPage(current string) (p compton.PageElement, stack *compton.FlexItemsElement) {
	p = compton.Page(current)
	p.RegisterStyles(compton_styles.VangoghStyles, "style/app.css")

	stack = compton.FlexItems(p, direction.Column)
	p.Append(stack)

	return p, stack
}
