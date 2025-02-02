package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/redux"
)

func Changelog(id string, rdx redux.Readable) compton.PageElement {

	// empty changelog is handled below, no need to check for existence here
	changelog, _ := rdx.GetAllValues(vangogh_integration.ChangelogProperty, id)

	s := compton_fragments.ProductSection(compton_data.ChangelogSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if len(changelog) == 0 {
		fs := compton.Fspan(s, "Changelog is not available for this product").
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(s, fs))
	}

	for _, log := range changelog {
		pageStack.Append(compton.Text(log))
	}

	return s
}
