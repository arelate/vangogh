package compton_fragments

import (
	"github.com/boggydigital/compton"
)

func DistributorNavLinks(r compton.Registrar, current string) compton.Element {

	distNavLinks := compton.NavLinks(r)

	distNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/gog",
		Title:       "GOG",
		IconElement: compton.SvgUse(r, compton.LetterG),
		Selected:    current == "GOG",
	})

	return distNavLinks
}
