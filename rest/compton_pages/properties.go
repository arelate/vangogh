package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/redux"
)

func Information(id string, rdx redux.Readable) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.InformationSection)

	grid := compton.GridItems(s).JustifyContent(align.Center)
	s.Append(grid)

	for _, pp := range compton_fragments.ProductProperties(s, id, rdx, compton_data.ProductProperties...) {
		grid.Append(pp)
	}

	return s
}
