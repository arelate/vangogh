package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func SearchLinks(r compton.Registrar, current string) compton.Element {

	searchNavLinks := compton.NavLinks(r)

	searchScopes := compton_data.SearchScopes()

	for _, scope := range compton_data.SearchOrder {
		searchNavLinks.AppendLink(r, &compton.NavTarget{
			Href:     "/search?" + searchScopes[scope],
			Title:    scope,
			Selected: current == scope,
		})
	}

	return searchNavLinks
}
