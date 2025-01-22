package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
)

func ProductSectionsLinks(r compton.Registrar, sections []string) compton.Element {

	sectionLinks := make(map[string]string)
	sectionsOrder := make([]string, 0, len(sections))
	for _, s := range sections {
		title := compton_data.SectionTitles[s]
		sectionLinks[title] = "#" + title
		sectionsOrder = append(sectionsOrder, title)
	}

	targets := compton.TextLinks(sectionLinks, "", sectionsOrder...)

	psl := compton.NavLinksTargets(r, targets...)
	psl.SetId("product-section-links")

	return psl

}
