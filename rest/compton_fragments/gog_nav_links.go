package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func GogNavLinks(r compton.Registrar, current string) compton.Element {

	gogNavLinks := compton.NavLinks(r)

	for _, gogSection := range compton_data.AllGogSectionUrls() {
		gogNavLinks.AppendLink(r, &compton.NavTarget{
			Href:        gogSection,
			Title:       compton_data.GogSectionTitles[gogSection],
			IconElement: compton.SvgUse(r, compton_data.GogSectionSymbols[gogSection]),
			Selected:    current == compton_data.GogSectionTitles[gogSection],
		})
	}

	return gogNavLinks
}
