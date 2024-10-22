package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func ProductSection(section string) compton.PageElement {

	title := compton_data.SectionTitles[section]
	ifc := compton.IframeExpandContent(section, title)

	if style, ok := compton_data.SectionStyles[section]; ok && style != nil {
		ifc.AppendStyle("style-section", style)
	}

	return ifc
}
