package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func AppNavLinks(r compton.Registrar, current string) compton.Element {

	appNavLinks := compton.NavLinks(r)
	appNavLinks.SetAttribute("style", "view-transition-name:primary-nav")

	appNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/updates",
		Title:       compton_data.AppNavUpdates,
		IconElement: compton.SvgUse(r, compton.Sparkle),
		//Symbol:   compton.Sparkle,
		Selected: current == compton_data.AppNavUpdates,
	})

	appNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/search",
		Title:       compton_data.AppNavSearch,
		IconElement: compton.SvgUse(r, compton.Search),
		//Symbol:   compton.Search,
		Selected: current == compton_data.AppNavSearch,
	})

	return appNavLinks
}
