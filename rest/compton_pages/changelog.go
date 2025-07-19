package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
)

func Changelog(id string, rdx redux.Readable) compton.PageElement {

	//s := compton_fragments.ProductSection(compton_data.ChangelogSection)
	var pageTitle string
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		pageTitle = title
	}

	p := compton.Page(pageTitle)

	p.RegisterStyles(compton_styles.Styles, "changelog.css")

	// tinting document background color to the representative product color
	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if repColor, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && repColor != issa.NeutralRepColor {
			p.SetAttribute("style", "--c-rep:"+repColor)
		}
	}

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(compton.FICenter(p, pageStack))

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	headingRow := compton.FlexItems(p, direction.Column).RowGap(size.XSmall)

	heading := compton.Heading(1)
	heading.Append(compton.Fspan(p, pageTitle).TextAlign(align.Center))
	heading.SetAttribute("style", "view-transition-name:product-title-"+id)
	headingRow.Append(heading)

	subHeading := compton.Heading(2)
	subHeading.Append(compton.Fspan(p, "Changelog").
		TextAlign(align.Center).
		ForegroundColor(color.RepGray))
	headingRow.Append(subHeading)

	pageStack.Append(compton.FICenter(p, headingRow))

	if changelog, ok := rdx.GetAllValues(vangogh_integration.ChangelogProperty, id); !ok || len(changelog) == 0 {
		fs := compton.Fspan(p, "Changelog is not available for this product").
			ForegroundColor(color.RepGray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(p, fs))
	} else {
		changelogStack := compton.FlexItems(p, direction.Column).
			AlignItems(align.Start).
			RowGap(size.Normal).
			MaxWidth(size.MaxWidth)
		changelogStack.AddClass("changelog-content")

		pageStack.Append(changelogStack)

		for _, log := range changelog {
			changelogStack.Append(compton.Text(log))
		}
	}

	pageStack.Append(compton.Br(),
		compton.Footer(p, "Bonjour d'Arles", "https://github.com/arelate"))

	return p
}
