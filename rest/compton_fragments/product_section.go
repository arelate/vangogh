package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/elements/iframe_expand"
)

func ProductSection(section string) compton.PageElement {

	title := compton_data.SectionTitles[section]
	ifc := iframe_expand.IframeExpandContent(section, title)

	if style, ok := compton_data.SectionStyles[section]; ok && style != nil {
		ifc.AppendStyle("style-section", style)
	}

	return ifc
}
