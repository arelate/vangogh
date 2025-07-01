package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
)

func AppPage(current string) (p compton.PageElement, stack *compton.FlexItemsElement) {
	p = compton.Page(current)
	p.AppendIcon()
	p.AppendManifest()

	stack = compton.FlexItems(p, direction.Column)
	p.Append(stack)

	return p, stack
}
