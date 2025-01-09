package compton_fragments

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/kevlar"
)

func SetTint(id string, p compton.Element, rdx kevlar.ReadableRedux) {
	if repColor, ok := rdx.GetLastVal(vangogh_local_data.RepImageColorProperty, id); ok && repColor != issa.NeutralRepColor {
		p.SetAttribute("style", "background-color:color-mix(in display-p3,"+repColor+" var(--cma),var(--c-background))")
	}
}
