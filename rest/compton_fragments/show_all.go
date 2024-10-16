package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/inputs"
)

func ShowMoreButton(r compton.Registrar, title, href string) compton.Element {

	showAllLink := els.A(href)

	button := inputs.InputValue(r, input_types.Submit, title)
	showAllLink.Append(button)

	return flex_items.Center(r, showAllLink)

}
