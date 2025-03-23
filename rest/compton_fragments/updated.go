package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
)

func Updated(r compton.Registrar, updated string) compton.Element {

	updatedTitle := compton.Fspan(r, "Updated: ").ForegroundColor(color.Gray)
	updatedValue := compton.Fspan(r, updated)

	return compton.FICenter(r, updatedTitle, updatedValue).FontSize(size.XXSmall).ColumnGap(size.Small)
}
