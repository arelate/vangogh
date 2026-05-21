package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
)

func Error(err error) compton.PageElement {

	const pageTitle = "Error"

	p := compton.Page(pageTitle)
	pageStack := compton.FlexItems(p, direction.Column).RowGap(size.Large)
	p.Append(pageStack)

	headingsStack := compton.FlexItems(p, direction.Column).RowGap(size.Small)
	titleHeading := compton.H2Text(pageTitle)

	headingsStack.Append(titleHeading)

	pageStack.Append(compton.Div(), compton.FICenter(p, headingsStack))

	pageStack.Append(compton.FICenter(p, compton.SpanText(err.Error())))

	pageStack.Append(compton.FICenter(p, compton_fragments.GitHubLink(p)))

	return p
}
