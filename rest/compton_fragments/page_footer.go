package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
)

func PageFooter(r compton.Registrar) compton.Element {

	footerRow := compton.FlexItems(r, direction.Row).ColumnGap(size.Normal).JustifyContent(align.Center)

	footerRow.Append(GitHubLink(r))
	logoutLink := compton.A("/logout")
	logoutLink.Append(compton.Fspan(r, "Logout").FontSize(size.XSmall).ForegroundColor(color.Blue).FontWeight(font_weight.Bolder))
	footerRow.Append(logoutLink)

	return footerRow
}
