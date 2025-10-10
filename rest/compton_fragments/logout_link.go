package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
)

func LogoutLink(r compton.Registrar) compton.Element {

	logoutLink := compton.A("/logout")
	logoutLink.Append(compton.Fspan(r, "Logout").FontSize(size.XSmall).ForegroundColor(color.Blue).FontWeight(font_weight.Bolder))

	return logoutLink
}
