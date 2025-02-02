package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func Updates(sections []string,
	updates map[string][]string,
	sectionTitles map[string]string,
	updateTotals map[string]int,
	updated string,
	rdx redux.Readable) compton.PageElement {

	current := compton_data.AppNavUpdates
	p, pageStack := compton_fragments.AppPage(current)
	p.RegisterStyles(compton_styles.Styles, "product-card.css")
	p.AppendSpeculationRules("/product?id=*")

	/* Nav stack = App navigation + Show all + (popup) Updates sections shortcuts */

	topLevelNav := []compton.Element{compton_fragments.AppNavLinks(p, current)}

	var showAll compton.Element
	if hasMoreItems(sections, updates, updateTotals) {
		showAll = compton_fragments.Button(p, "Show all", "?show-all=true")
		topLevelNav = append(topLevelNav, showAll)
	}

	if sectionNav := compton.SectionsLinks(p, sections, sectionTitles); sectionNav != nil {
		showToc := compton.InputValue(p, input_types.Button, compton.SectionLinksTitle)
		pageStack.Append(compton.Attach(p, showToc, sectionNav))

		topLevelNav = append(topLevelNav, showToc, sectionNav)
	}

	pageStack.Append(compton.FICenter(p, topLevelNav...))

	/* Show All... button */

	/* Updates sections */

	for ii, section := range sections {

		ids := updates[section]

		sectionTitle := sectionTitles[section]

		sectionHeading := compton.DSTitle(p, sectionTitle)

		dsSection := compton.DSLarge(p, sectionHeading, true).
			BackgroundColor(color.Highlight).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset).
			SummaryRowGap(size.XXSmall)

		cf := compton.NewCountFormatter(
			compton_data.SingleItemTemplate,
			compton_data.ManyItemsSinglePageTemplate,
			compton_data.ManyItemsManyPagesTemplate)

		dsSection.AppendSummary(cf.TitleElement(p, 0, len(ids), updateTotals[section]))

		dsSection.SetId(sectionTitle)
		dsSection.SetTabIndex(ii + 1)
		pageStack.Append(dsSection)

		sectionStack := compton.FlexItems(p, direction.Column)
		dsSection.Append(sectionStack)

		productsList := compton_fragments.ProductsList(p, ids, 0, len(ids), rdx)
		sectionStack.Append(productsList)
	}

	/* Show All.. button at the bottom of the page */

	if showAll != nil {
		pageStack.Append(showAll)
	}

	/* Last Updated section */

	pageStack.Append(compton.Br(), compton_fragments.Updated(p, updated))

	/* Standard app footer */

	pageStack.Append(compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}

func hasMoreItems(sections []string, updates map[string][]string, updateTotals map[string]int) bool {
	for _, section := range sections {
		if len(updates[section]) < updateTotals[section] {
			return true
		}
	}
	return false
}
