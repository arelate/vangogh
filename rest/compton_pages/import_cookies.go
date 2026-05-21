package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
)

func ImportCookiesInput() compton.PageElement {

	const pageTitle = "Import GOG.com cookies"

	p := compton.Page(pageTitle)
	pageStack := compton.FlexItems(p, direction.Column).RowGap(size.Large)
	p.Append(pageStack)

	headingsStack := compton.FlexItems(p, direction.Column).RowGap(size.Small)
	titleHeading := compton.H2Text(pageTitle)

	headingsStack.Append(titleHeading)

	pageStack.Append(compton.Div(), compton.FICenter(p, headingsStack))

	instructions := compton.Div()
	guideLink := compton.A("https://github.com/boggydigital/coost/blob/main/README.md")
	guideLink.Append(compton.Fspan(p, "this guide").FontWeight(font_weight.Bolder).ForegroundColor(color.Blue))

	instructions.Append(
		compton.SpanText("Please follow "),
		guideLink,
		compton.SpanText(" to copy GOG.com cookies and paste below."))

	pageStack.Append(compton.FICenter(p, instructions))

	form := compton.Form("/import-cookies", "POST")

	swColumn := compton.FlexItems(p, direction.Column).
		AlignContent(align.Center).
		JustifyContent(align.Start).
		Width(size.XXXLarge)
	form.Append(swColumn)

	newValueInput := compton.Input(p, input_types.Text)
	newValueInput.SetName("import-cookies")
	newValueInput.SetPlaceholder("Paste GOG.com cookies here")

	swColumn.Append(newValueInput)

	applyNavLink := compton.NavLinks(p)
	applyNavLink.AppendSubmitLink(p, &compton.NavTarget{
		Href:  "#",
		Title: "Import",
	})
	swColumn.Append(applyNavLink)

	pageStack.Append(compton.FICenter(p, form))

	pageStack.Append(compton.FICenter(p, compton_fragments.GitHubLink(p)))

	return p
}
