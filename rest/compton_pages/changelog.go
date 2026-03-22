package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
)

func Changelog(pageTitle string, changelog string) compton.PageElement {

	p := compton.Page(pageTitle)

	p.RegisterStyles(compton_styles.Styles, "changelog.css")

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(compton.FICenter(p, pageStack))

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	headingRow := compton.FlexItems(p, direction.Column).RowGap(size.XSmall)

	heading := compton.Heading(2)
	heading.Append(compton.Fspan(p, pageTitle).TextAlign(align.Center))
	headingRow.Append(heading)

	subHeading := compton.Heading(3)
	subHeading.Append(compton.Fspan(p, "Changelog").
		TextAlign(align.Center).
		ForegroundColor(color.Gray))
	headingRow.Append(subHeading)

	pageStack.Append(compton.FICenter(p, headingRow))

	changelogStack := compton.FlexItems(p, direction.Column).
		AlignItems(align.Start).
		RowGap(size.Normal).
		MaxWidth(size.MaxWidth)
	changelogStack.AddClass("changelog-content")

	pageStack.Append(changelogStack)

	changelogStack.Append(compton.Text(changelog))

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}
