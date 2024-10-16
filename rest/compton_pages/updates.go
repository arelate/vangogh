package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/gaugin_elements/product_labels"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/details_summary"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/nav_links"
	"github.com/boggydigital/compton/page"
	"github.com/boggydigital/kevlar"
)

func Updates(sections []string,
	updates map[string][]string,
	sectionTitles map[string]string,
	updateTotals map[string]int,
	updated string,
	rdx kevlar.ReadableRedux) *page.PageElement {

	current := compton_data.AppNavUpdates
	p, pageStack := compton_fragments.AppPage(current)
	p.AppendStyle(product_labels.StyleProductLabels)

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

	sectionTargets := nav_links.TextLinks(sectionLinks, "", order...)

	sectionNav := nav_links.NavLinksTargets(p, sectionTargets...)
	pageStack.Append(flex_items.Center(p, appNavLinks, sectionNav))

	/* Show All... button */

	var showAll compton.Element
	if hasMoreItems(sections, updates, updateTotals) {
		showAll = compton_fragments.ShowMoreButton(p, "Show all", "?show-all=true")
		pageStack.Append(showAll)
	}

	/* Updates sections */

	for _, section := range sections {

		ids := updates[section]

		sectionTitle := sectionTitles[section]

		sectionHeading := compton_fragments.DetailsSummaryTitle(p, sectionTitle)

		sectionDetailsToggle := details_summary.
			Larger(p, sectionHeading, true).
			BackgroundColor(color.Highlight).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset).
			SummaryRowGap(size.XXSmall)

		itemsCount := compton_fragments.ItemsCount(p, 0, len(ids), updateTotals[section])
		sectionDetailsToggle.AppendSummary(itemsCount)

		sectionDetailsToggle.SetId(sectionTitle)
		pageStack.Append(sectionDetailsToggle)

		sectionStack := flex_items.FlexItems(p, direction.Column)
		sectionDetailsToggle.Append(sectionStack)

		//sectionStack.Append(itemsCount)

		productsList := compton_fragments.ProductsList(p, ids, 0, len(ids), rdx)
		sectionStack.Append(productsList)
	}

	/* Show All.. button at the bottom of the page */

	if showAll != nil {
		pageStack.Append(showAll)
	}

	/* Last Updated section */

	pageStack.Append(els.Br(), compton_fragments.Updated(p, updated))

	/* Standard app footer */

	pageStack.Append(compton_fragments.Footer(p))

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
