package compton_pages

import (
	"maps"
	"slices"
	"strconv"

	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

const filterSearchTitle = "Filter & search"

func Search(query map[string][]string, ids []string, from, to int, rdx redux.Readable, permissions ...author.Permission) compton.PageElement {

	query = permittedQuery(query, permissions...)

	p, pageStack := compton_fragments.AppPage(compton_data.AppNavSearch)

	p.AppendSpeculationRules(compton.SpeculationRulesConservativeEagerness, "/*")

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	/* Nav stack = App navigation + Search shortcuts */

	appNavLinks := compton_fragments.AppNavLinks(p, compton_data.AppNavSearch)

	searchScope := compton_data.SearchScopeFromQuery(query)
	searchLinks := compton_fragments.SearchLinks(p, searchScope, permissions...)

	pageStack.Append(compton.FICenter(p, appNavLinks, searchLinks).ColumnGap(size.Small))

	/* Filter & Search details */

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

		resultsBadge := compton.BadgeText(p, cf.Title(from, to, len(ids)), color.Foreground).FontSize(size.XXSmall)
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

	filterSearchDetails.Append(compton_fragments.SearchForm(p, query, queryFrow, rdx, permissions...))
	pageStack.Append(filterSearchDetails)

	if queryFrow != nil {
		pageStack.Append(compton.FICenter(p, queryFrow))
	}

	/* Search results product cards */

	if len(ids) > 0 {
		productsList := compton_fragments.ProductsList(p, ids, from, to, rdx, false, permissions...)
		pageStack.Append(productsList)
	}

	/* Show more... button */

	if to < len(ids) {
		query["from"] = []string{strconv.Itoa(to)}
		enq := compton_data.EncodeQuery(query)

		backToTopNavLinks := compton.NavLinks(p)
		backToTopNavLinks.AppendLink(p, &compton.NavTarget{Href: "#_top", Title: "Back to top"})

		nextPageNavLink := compton.NavLinks(p)
		nextPageNavLink.AppendLink(p, &compton.NavTarget{Href: "/search?" + enq, Title: "Next page"})

		pageStack.Append(compton.FICenter(p, backToTopNavLinks, nextPageNavLink).ColumnGap(size.Small))
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}

func permittedQuery(query map[string][]string, permissions ...author.Permission) map[string][]string {
	permQuery := make(map[string][]string, len(query))

	for p, v := range query {

		if prm, ok := compton_data.PropertyPermissions[p]; ok && !slices.Contains(permissions, prm) {
			continue
		}

		permQuery[p] = v

	}

	return permQuery
}
