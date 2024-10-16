package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/elements/nav_links"
)

func AppNavLinks(r compton.Registrar, current string) compton.Element {
	targets := nav_links.TextLinks(
		compton_data.AppNavLinks,
		current,
		compton_data.AppNavOrder...)
	nav_links.SetIcons(targets, compton_data.AppNavIcons)

	return nav_links.NavLinksTargets(r, targets...)
}
