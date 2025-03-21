package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func ProductDetails(id string, rdx redux.Readable) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.ProductDetailsSection)

	if properties := compton_fragments.ProductProperties(s, id, rdx); properties != nil {
		s.Append(properties)
	}

	return s
}
