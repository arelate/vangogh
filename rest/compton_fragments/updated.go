package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
)

func Updated(r compton.Registrar, updated string) compton.Element {

	updatedTitle := fspan.Text(r, "Updated: ").ForegroundColor(color.Gray)
	updatedValue := fspan.Text(r, updated)

	return flex_items.Center(r, updatedTitle, updatedValue).FontSize(size.Small).ColumnGap(size.Small)
}
