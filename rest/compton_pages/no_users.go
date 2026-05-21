package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
)

func NoUsers() compton.PageElement {

	const (
		pageTitle = "No users found"
	)

	p := compton.Page(pageTitle)
	pageStack := compton.FlexItems(p, direction.Column).RowGap(size.Large)
	p.Append(pageStack)

	headingsStack := compton.FlexItems(p, direction.Column).RowGap(size.Small)
	titleHeading := compton.H2Text(pageTitle)

	headingsStack.Append(titleHeading)

	pageStack.Append(compton.Div(), compton.FICenter(p, headingsStack))

	suggestion := compton.Div()
	guideLink := compton.A("https://github.com/arelate/vangogh/wiki/Authorization-and-user-management#creating-vangogh-users")
	guideLink.Append(compton.Fspan(p, "this guide").FontWeight(font_weight.Bolder).ForegroundColor(color.Blue))

	suggestion.Append(
		compton.SpanText("Please follow "),
		guideLink,
		compton.SpanText(" to create users."))

	pageStack.Append(compton.FICenter(p, suggestion))

	pageStack.Append(compton.FICenter(p, compton_fragments.GitHubLink(p)))

	return p
}
