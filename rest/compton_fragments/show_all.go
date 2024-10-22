package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/input_types"
)

func ShowMoreButton(r compton.Registrar, title, href string) compton.Element {

	showAllLink := compton.A(href)

	button := compton.InputValue(r, input_types.Submit, title)
	showAllLink.Append(button)

	return compton.FICenter(r, showAllLink)

}
