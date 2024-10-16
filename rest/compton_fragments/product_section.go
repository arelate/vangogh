package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton/elements/iframe_expand"
	"github.com/boggydigital/compton/page"
)

func ProductSection(section string) *page.PageElement {

	title := compton_data.SectionTitles[section]
	ifc := iframe_expand.IframeExpandContent(section, title)

	if style, ok := compton_data.SectionStyles[section]; ok && style != nil {
		ifc.AppendStyle(style)
	}

	return ifc
}
