package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
)

func Updates(sections []string,
	updates map[string][]string,
	sectionTitles map[string]string,
	updateTotals map[string]int,
	updated string,
	rdx kevlar.ReadableRedux) compton.PageElement {

	current := compton_data.AppNavUpdates
	p, pageStack := compton_fragments.AppPage(current)
	p.RegisterStyles(compton_styles.Styles, "product-card.css")

	/* Nav stack = App navigation + Updates sections shortcuts */

	appNavLinks := compton_fragments.AppNavLinks(p, current)

	/* Ordered list of Updates sections */

	order := make([]string, 0, len(sections))
	sectionLinks := make(map[string]string)
	for _, section := range sections {
		st := sectionTitles[section]
		sectionLinks[st] = "#" + st
		order = append(order, st)
	}

	sectionTargets := compton.TextLinks(sectionLinks, "", order...)

	sectionNav := compton.NavLinksTargets(p, sectionTargets...)
	pageStack.Append(compton.FICenter(p, appNavLinks, sectionNav))

	/* Show All... button */

	var showAll compton.Element
	if hasMoreItems(sections, updates, updateTotals) {
		showAll = compton_fragments.Button(p, "Show all", "?show-all=true")
		pageStack.Append(showAll)
	}

	/* Updates sections */

	for _, section := range sections {

		ids := updates[section]

		sectionTitle := sectionTitles[section]

		sectionHeading := compton.DSTitle(p, sectionTitle)

		sectionDetailsToggle := compton.DSLarge(p, sectionHeading, true).
			BackgroundColor(color.Highlight).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset).
			SummaryRowGap(size.XXSmall)

		cf := compton.NewCountFormatter(
			compton_data.SingleItemTemplate,
			compton_data.ManyItemsSinglePageTemplate,
			compton_data.ManyItemsManyPagesTemplate)

		sectionDetailsToggle.AppendSummary(cf.TitleElement(p, 0, len(ids), updateTotals[section]))

		sectionDetailsToggle.SetId(sectionTitle)
		pageStack.Append(sectionDetailsToggle)

		sectionStack := compton.FlexItems(p, direction.Column)
		sectionDetailsToggle.Append(sectionStack)

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
