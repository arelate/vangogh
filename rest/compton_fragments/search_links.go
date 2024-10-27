package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func SearchLinks(r compton.Registrar, current string) compton.Element {

	searchLinks := make(map[string]string)
	for dst, q := range compton_data.SearchScopes() {
		searchLinks[dst] = "/search?" + q
	}

	targets := compton.TextLinks(
		searchLinks,
		current,
		compton_data.SearchOrder...)

	return compton.NavLinksTargets(r, targets...)
}
