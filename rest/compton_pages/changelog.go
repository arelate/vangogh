package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
	"strings"
)

func Changelog(id string, rdx redux.Readable) compton.PageElement {

	//s := compton_fragments.ProductSection(compton_data.ChangelogSection)
	var pageTitle string
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		pageTitle = strings.Join([]string{title, "Changelog"}, " ")
	}

	p := compton.Page(pageTitle)

	p.RegisterStyles(compton_styles.Styles, "changelog.css")

	// tinting document background color to the representative product color
	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if repColor, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && repColor != issa.NeutralRepColor {
			p.SetAttribute("style", "--c-rep:"+repColor)
		}
	}

	pageStack := compton.FlexItems(p, direction.Column).
		RowGap(size.Large)
	p.Append(compton.FICenter(p, pageStack))

	heading := compton.Heading(1)
	heading.Append(compton.Fspan(p, pageTitle).TextAlign(align.Center))
	heading.SetAttribute("style", "view-transition-name:product-title-"+id)
	pageStack.Append(compton.FICenter(p, heading))

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

		pageStack.Append(changelogStack)

		for _, log := range changelog {
			changelogStack.Append(compton.Text(log))
		}
	}

	return p
}
