package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/page"
)

func AppPage(current string) (p compton.PageElement, stack *flex_items.FlexItemsElement) {
	p = page.Page(current).
		AppendStyle("style-app", compton_styles.AppStyle)

	stack = flex_items.FlexItems(p, direction.Column)
	p.Append(stack)

	return p, stack
}
