package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
)

func GitHubLink(r compton.Registrar) compton.Element {
	gitHubLink := compton.A("https://github.com/arelate")
	gitHubLink.Append(compton.Fspan(r, "Bonjour d'Arles").FontWeight(font_weight.Bolder).FontSize(size.XSmall).ForegroundColor(color.Cyan))
	return gitHubLink
}
