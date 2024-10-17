package compton_fragments

import (
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/page"
)

func AppPage(current string) (p *page.PageElement, stack *flex_items.FlexItemsElement) {
	p = page.Page(current).
		AppendStyle(compton_styles.AppStyle).
		AppendManifest().
		AppendFavIcon()

	stack = flex_items.FlexItems(p, direction.Column)
	p.Append(stack)

	return p, stack
}
