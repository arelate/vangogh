package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func AppNavLinks(r compton.Registrar, current string) compton.Element {

	appNavLinks := compton.NavLinks(r)
	appNavLinks.SetAttribute("style", "view-transition-name:app-nav-links")

	appNavLinks.AppendLink(r, &compton.NavTarget{
		Href:     "/updates",
		Title:    compton_data.AppNavUpdates,
		Symbol:   compton.Sparkle,
		Selected: current == compton_data.AppNavUpdates,
	})

	appNavLinks.AppendLink(r, &compton.NavTarget{
		Href:     "/search",
		Title:    compton_data.AppNavSearch,
		Symbol:   compton.Search,
		Selected: current == compton_data.AppNavSearch,
	})

	return appNavLinks
}
