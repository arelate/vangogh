package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
	"strings"
)

func Changelog(id string, rdx redux.Readable) compton.PageElement {

	//s := compton_fragments.ProductSection(compton_data.ChangelogSection)
	var pageTitle string
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		pageTitle = strings.Join([]string{title, "Changelog"}, " ")
	}

	p, pageStack := compton_fragments.AppPage(pageTitle)
	p.RegisterStyles(compton_styles.Styles, "changelog.css")

	pageStack.Append(compton.FICenter(p, compton.HeadingText(pageTitle, 1)))

	if changelog, ok := rdx.GetAllValues(vangogh_integration.ChangelogProperty, id); !ok || len(changelog) == 0 {
		fs := compton.Fspan(p, "Changelog is not available for this product").
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(p, fs))
	} else {
		for _, log := range changelog {
			pageStack.Append(compton.Text(log))
		}
	}

	return p
}
