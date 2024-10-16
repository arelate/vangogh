package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
	"github.com/boggydigital/kevlar"
)

func Changelog(id string, rdx kevlar.ReadableRedux) compton.Element {

	// empty changelog is handled below, no need to check for existence here
	changelog, _ := rdx.GetAllValues(vangogh_local_data.ChangelogProperty, id)

	s := compton_fragments.ProductSection(compton_data.ChangelogSection)

	pageStack := flex_items.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if len(changelog) == 0 {
		fs := fspan.Text(s, "Changelog is not available for this product").
			ForegroundColor(color.Gray)
		pageStack.Append(flex_items.Center(s, fs))
	}

	for _, log := range changelog {
		pageStack.Append(els.Text(log))
	}

	return s
}
