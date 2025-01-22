package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"golang.org/x/exp/maps"
	"slices"
	"strconv"
)

const filterSearchTitle = "Filter & search"

func Search(query map[string][]string, ids []string, from, to int, rdx kevlar.ReadableRedux) compton.PageElement {

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

	filterSearchHeading := compton.DSTitle(p, filterSearchTitle)

	filterSearchDetails := compton.DSLarge(p, filterSearchHeading, len(query) == 0).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	if len(query) > 0 {

		cf := compton.NewCountFormatter(
			compton_data.SingleItemTemplate,
			compton_data.ManyItemsSinglePageTemplate,
			compton_data.ManyItemsManyPagesTemplate)

		filterSearchDetails.AppendSummary(cf.TitleElement(p, from, to, len(ids)))
	}

	var queryFrow *compton.FrowElement
	if len(query) > 0 {
		queryFrow = compton.Frow(p).FontSize(size.Small)
		fq := compton_fragments.FormatQuery(query, rdx)
		props := maps.Keys(query)
		slices.Sort(props)
		for _, prop := range props {
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
		productsList := compton_fragments.ProductsList(p, ids, from, to, rdx)
		pageStack.Append(productsList)
	}

	/* Show more... button */

	if to < len(ids) {
		query["from"] = []string{strconv.Itoa(to)}
		enq := compton_data.EncodeQuery(query)

		href := "/search?" + enq

		pageStack.Append(compton_fragments.Button(p, "Next page", href))
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(), compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}
