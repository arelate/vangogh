package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
)

func Footer(r compton.Registrar) compton.Element {

	link := compton.A("https://github.com/arelate")
	link.Append(compton.Fspan(r, "Arles").FontWeight(font_weight.Bolder))

	row := compton.FICenter(r, compton.Fspan(r, "👋"), compton.Fspan(r, "from"), link, compton.Fspan(r, "🇫🇷")).
		ColumnGap(size.XSmall).
		FontSize(size.Small)

	return row
}
