package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/elements/nav_links"
)

func SearchLinks(r compton.Registrar, current string) compton.Element {

	searchLinks := make(map[string]string)
	for dst, q := range compton_data.SearchScopes() {
		searchLinks[dst] = compton_data.SearchPath + "?" + q
	}

	targets := nav_links.TextLinks(
		searchLinks,
		current,
		compton_data.SearchOrder...)

	return nav_links.NavLinksTargets(r, targets...)
}
