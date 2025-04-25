package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"maps"
	"slices"
	"strconv"
)

const filterSearchTitle = "Filter & search"

func Search(query map[string][]string, ids []string, from, to int, rdx redux.Readable) compton.PageElement {

	current := compton_data.AppNavSearch
	p, pageStack := compton_fragments.AppPage(current)
	p.RegisterStyles(compton_styles.Styles, "product-card.css")
	p.AppendSpeculationRules("/product?id=*")

	/* Nav stack = App navigation + Search shortcuts */

	appNavLinks := compton_fragments.AppNavLinks(p, current)

	searchScope := compton_data.SearchScopeFromQuery(query)
	searchLinks := compton_fragments.SearchLinks(p, searchScope)

	pageStack.Append(compton.FICenter(p, appNavLinks, searchLinks))

	/* Filter & Search details */

	//filterSearchHeading := compton.DSTitle(p, filterSearchTitle)

	filterSearchDetails := compton.DSLarge(p, filterSearchTitle, len(query) == 0).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	if len(query) > 0 {

		cf := compton.NewCountFormatter(
			compton_data.SingleItemTemplate,
			compton_data.ManyItemsSinglePageTemplate,
			compton_data.ManyItemsManyPagesTemplate)

		resultsBadge := compton.Badge(p, cf.Title(from, to, len(ids)), color.Background, color.Foreground)
		filterSearchDetails.AppendBadges(resultsBadge)
	}

	var queryFrow *compton.FrowElement
	if len(query) > 0 {
		queryFrow = compton.Frow(p).FontSize(size.XSmall)
		fq := compton_fragments.FormatQuery(query, rdx)
		props := maps.Keys(query)
		sortedPropes := slices.Sorted(props)
		for _, prop := range sortedPropes {
			vals := fq[prop]
			queryFrow.PropVal(compton_data.PropertyTitles[prop], vals...)
		}
		queryFrow.LinkColor("Clear", "/search", color.Blue)
	}

	filterSearchDetails.Append(compton_fragments.SearchForm(p, query, queryFrow, rdx))
	pageStack.Append(filterSearchDetails)

	if queryFrow != nil {
		pageStack.Append(compton.FICenter(p, queryFrow))
	}

	/* Search results product cards */

	if len(ids) > 0 {
		productsList := compton_fragments.ProductsList(p, ids, from, to, rdx, false)
		pageStack.Append(productsList)
	}

	/* Show more... button */

	if to < len(ids) {
		query["from"] = []string{strconv.Itoa(to)}
		enq := compton_data.EncodeQuery(query)

		nextPageNavLink := compton.NavLinks(p)
		nextPageNavLink.AppendLink(p, &compton.NavTarget{Href: "/search?" + enq, Title: "Next page"})

		pageStack.Append(nextPageNavLink)
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(), compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}
