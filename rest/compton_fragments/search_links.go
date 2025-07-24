package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func SearchLinks(r compton.Registrar, current string) compton.Element {

	searchNavLinks := compton.NavLinks(r)
	searchNavLinks.SetAttribute("style", "view-transition-name:secondary-nav")

	searchScopes := compton_data.SearchScopes()

	for _, scope := range compton_data.SearchOrder {
		searchLink := searchNavLinks.AppendLink(r, &compton.NavTarget{
			Href:     "/search?" + searchScopes[scope],
			Title:    scope,
			Selected: current == scope,
			Symbol:   compton_data.SearchScopesSymbols[scope],
		})
		if current == scope {
			searchLink.SetAttribute("style", "view-transition-name:current-search-link")
		}
	}

	return searchNavLinks
}
