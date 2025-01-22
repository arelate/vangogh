package compton_fragments

import (
	"github.com/boggydigital/compton"
)

func SectionsLinks(r compton.Registrar, sections []string, sectionTitles map[string]string) compton.Element {

	sectionLinks := make(map[string]string)
	sectionsOrder := make([]string, 0, len(sections))
	for _, s := range sections {
		title := sectionTitles[s]
		sectionLinks[title] = "#" + title
		sectionsOrder = append(sectionsOrder, title)
	}

	targets := compton.TextLinks(sectionLinks, "", sectionsOrder...)

	psl := compton.NavLinksTargets(r, targets...)
	psl.SetId("section-links")

	return psl

}
