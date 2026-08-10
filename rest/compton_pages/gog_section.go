package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func GogSection(section string, rdx redux.Readable) compton.PageElement {

	p, pageStack := compton_fragments.AppPage("GOG Search")

	p.AppendSpeculationRules(compton.SpeculationRulesConservativeEagerness, "/*")

	/* Nav stack = App navigation + Search shortcuts */

	pageStack.Append(compton.FICenter(p,
		compton_fragments.DistributorNavLinks(p, "GOG"),
		compton_fragments.GogNavLinks(p, section)))

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}
