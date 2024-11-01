package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/kevlar"
)

func Properties(id string, rdx kevlar.ReadableRedux) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.PropertiesSection)

	if properties := compton_fragments.ProductProperties(s, id, rdx); properties != nil {
		s.Append(properties)
	}

	return s
}
