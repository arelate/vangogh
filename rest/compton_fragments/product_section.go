package compton_fragments

import (
	"strings"

	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func ProductSection(section string, id string, rdx redux.Readable) compton.PageElement {

	sectionId := strings.Replace(section, "/", "-", -1)

	title := compton_data.SectionTitles[section]
	ifc := compton.IframeExpandContent(sectionId, title)

	if style, ok := compton_data.SectionStyles[section]; ok && style != "" {
		ifc.RegisterStyles(compton_styles.Styles, style)
	}

	return ifc
}
