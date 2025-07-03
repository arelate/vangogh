package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func ProductSection(section string, id string, rdx redux.Readable) compton.PageElement {

	title := compton_data.SectionTitles[section]
	ifc := compton.IframeExpandContent(section, title)

	if style, ok := compton_data.SectionStyles[section]; ok && style != "" {
		ifc.RegisterStyles(compton_styles.Styles, style)
	}

	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if rc, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && rc != "" {
			ifc.SetAttribute("style", "--c-rep:"+rc)
		}
	}

	return ifc
}
