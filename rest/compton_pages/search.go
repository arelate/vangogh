package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"strconv"
)

const (
	filterSearchTitle = "Filter & search"
)

func Search(query map[string][]string, ids []string, from, to int, rdx kevlar.ReadableRedux) compton.PageElement {

	current := compton_data.AppNavSearch
	p, pageStack := compton_fragments.AppPage(current)
	p.RegisterStyles(compton_styles.VangoghStyles, "style/product-labels.css")

	/* Nav stack = App navigation + Search shortcuts */

	appNavLinks := compton_fragments.AppNavLinks(p, current)

	searchScope := compton_data.SearchScopeFromQuery(query)
	searchLinks := compton_fragments.SearchLinks(p, searchScope)

	pageStack.Append(compton.FICenter(p, appNavLinks, searchLinks))

	/* Filter & Search details */

	filterSearchHeading := compton_fragments.DetailsSummaryTitle(p, filterSearchTitle)

	filterSearchDetails := compton.DSLarge(p, filterSearchHeading, len(query) == 0).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	if len(query) > 0 {

		itemsCount := compton_fragments.ItemsCount(p, from, to, len(ids))
		filterSearchDetails.AppendSummary(itemsCount)
	}

	searchQueryDisplay := compton_fragments.SearchQueryDisplay(query, p)

	filterSearchDetails.Append(compton_fragments.SearchForm(p, query, searchQueryDisplay, rdx))
	pageStack.Append(filterSearchDetails)

	if searchQueryDisplay != nil {
		pageStack.Append(searchQueryDisplay)
	}

	/* Search results product cards */

	if len(ids) > 0 {
		productsList := compton_fragments.ProductsList(p, ids, from, to, rdx)
		pageStack.Append(productsList)
	}

	/* Show more... button */

	if to < len(ids) {
		query["from"] = []string{strconv.Itoa(to)}
		enq := compton_data.EncodeQuery(query)

		href := "/search?" + enq

		pageStack.Append(compton_fragments.ShowMoreButton(p, "Next page", href))
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(), compton_fragments.Footer(p))

	return p
}
