package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
)

func DebugLink(r compton.Registrar, id string) compton.Element {
	debugLink := compton.A("/debug?id=" + id)
	debugLink.Append(compton.Fspan(r, "Debug").FontSize(size.XSmall).ForegroundColor(color.RepForeground).FontWeight(font_weight.Bolder))

	return debugLink
}
