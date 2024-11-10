package compton_fragments

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/kevlar"
)

func SetTintedBackgroundColor(id string, p compton.Element, rdx kevlar.ReadableRedux) {
	if repColor, ok := rdx.GetLastVal(vangogh_local_data.RepImageColorProperty, id); ok {
		p.SetAttribute("style", "background-color:color-mix(in display-p3,"+repColor+" var(--cma),var(--c-background))")
	}
}
