package compton_pages

import (
	"maps"
	"net/url"
	"slices"
	"strconv"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

const (
	SearchResultsLimit = 60 // divisible by 2,3,4,5,6
)

var gogSectionTitles = map[string]string{
	"/gog/search":   "Search",
	"/gog/owned":    "Owned",
	"/gog/catalog":  "Catalog",
	"/gog/wishlist": "Wishlist",
	"/gog/sale":     "Sale",
}

const filterSearchTitle = "Filter & search"

func GogSearch(sectionUrl string, query url.Values, rdx redux.Readable, permissions ...author.Permission) compton.PageElement {

	query = permittedQuery(query, permissions...)

	maps.Copy(query, searchQueryByUrl(sectionUrl))

	sectionTitle := "vangogh"
	if st, ok := gogSectionTitles[sectionUrl]; ok {
		sectionTitle = st
	}

	p, pageStack := compton_fragments.AppPage(sectionTitle)

	p.AppendSpeculationRules(compton.SpeculationRulesConservativeEagerness, "/*")

	/* Nav stack = App navigation + Search shortcuts */

	pageStack.Append(compton.FICenter(p,
		compton_fragments.DistributorNavLinks(p, "GOG"),
		compton_fragments.GogNavLinks(p, sectionTitle)))

	/* Filter & Search details */

	filterSearchDetails := compton.DSLarge(p, filterSearchTitle, len(query) == 0).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	ids, from, to, err := searchResults(query, rdx)
	if err != nil {
		return p.Error(err)
	}

	if len(query) > 0 {

		cf := compton.NewCountFormatter(
			compton_data.SingleItemTemplate,
			compton_data.ManyItemsSinglePageTemplate,
			compton_data.ManyItemsManyPagesTemplate)

		filterSearchDetails.AppendBadges(compton.Badges(p, &compton.FormattedBadge{
			Title: cf.Title(from, to, len(ids)),
			Icon:  compton.NoSymbol,
			Color: color.Gray,
		}))
	}

	var queryFrow *compton.FrowElement
	if len(query) > 0 {
		queryFrow = compton.Frow(p).FontSize(size.XSmall)
		fq := compton_fragments.FormatQuery(query, rdx)
		props := maps.Keys(query)
		sortedPropes := slices.Sorted(props)
		for _, prop := range sortedPropes {
			queryFrow.PropVal(compton_data.PropertyTitles[prop], fq[prop]...)
		}
		queryFrow.LinkColor("Clear", "/gog/search", color.Foreground)
	}

	filterSearchDetails.Append(compton_fragments.GogSearchForm(p, query, queryFrow, rdx, permissions...))
	pageStack.Append(filterSearchDetails)

	if queryFrow != nil {
		pageStack.Append(compton.FICenter(p, queryFrow))
	}

	/* Search results product cards */

	if len(ids) > 0 {
		productsList := compton_fragments.GogProductsList(p, ids, from, to, rdx, false, permissions...)
		pageStack.Append(productsList)
	} else if len(query) > 0 {
		pageStack.Append(compton.Br(), compton.FICenter(p,
			compton.Fspan(p, "Nothing found.").ForegroundColor(color.Foreground)))
	}

	/* Show more... button */

	if to < len(ids) {

		nextQuery := make(url.Values)

		if useOriginalQuery(sectionUrl) {
			maps.Copy(nextQuery, query)
		}

		nextQuery.Set(vangogh_integration.UrlFromParameter, strconv.Itoa(to))

		backToTopNavLinks := compton.NavLinks(p)
		backToTopNavLinks.AppendLink(p, &compton.NavTarget{Href: "#_top", Title: "Back to top"})

		nextPageNavLink := compton.NavLinks(p)
		nextPageNavLink.AppendLink(p, &compton.NavTarget{Href: sectionUrl + "?" + nextQuery.Encode(), Title: "Next page"})

		pageStack.Append(compton.FICenter(p, backToTopNavLinks, nextPageNavLink).ColumnGap(size.Small))
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(), compton_fragments.SyncStatus(p, rdx, permissions...))

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}

func searchResults(query url.Values, rdx redux.Readable) (ids []string, from int, to int, err error) {

	if len(query) == 0 {
		return nil, 0, 0, nil
	}

	sort := query.Get(vangogh_integration.UrlSortParameter)
	if sort == "" {
		sort = vangogh_integration.GogTitleProperty
	}
	desc := query.Get(vangogh_integration.UrlDescendingParameter) == "true"

	var found []string

	if isSortDescOnly(query) {
		found = slices.Collect(rdx.Keys(vangogh_integration.GogTitleProperty))
	} else {
		found = slices.Collect(rdx.Match(query))
	}

	ids, err = rdx.Sort(found, desc, sort, vangogh_integration.GogTitleProperty, vangogh_integration.GogProductTypeProperty)
	if err != nil {
		return nil, 0, 0, err
	}

	if fromStr := query.Get("from"); fromStr != "" {
		if from, err = strconv.Atoi(fromStr); err != nil {
			return nil, 0, 0, err
		}
	}

	if from > len(ids)-1 {
		from = 0
	}

	to = from + SearchResultsLimit
	if to > len(ids) {
		to = len(ids)
	} else if to+SearchResultsLimit > len(ids) {
		to = len(ids)
	}

	return ids, from, to, nil
}

func useOriginalQuery(sectionUrl string) bool {
	switch sectionUrl {
	case "/gog/search":
		return true
	default:
		return false
	}
}

func isSortDescOnly(q map[string][]string) bool {
	switch len(q) {
	case 0:
		return false
	case 1:
		_, okSort := q[vangogh_integration.UrlSortParameter]
		_, okDesc := q[vangogh_integration.UrlDescendingParameter]
		return okSort || okDesc
	case 2:
		_, okSort := q[vangogh_integration.UrlSortParameter]
		_, okDesc := q[vangogh_integration.UrlDescendingParameter]
		return okSort && okDesc
	case 3:
		_, okSort := q[vangogh_integration.UrlSortParameter]
		_, okDesc := q[vangogh_integration.UrlDescendingParameter]
		_, okFrom := q[vangogh_integration.UrlFromParameter]
		return okSort && okDesc && okFrom
	default:
		return false
	}

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

func searchQueryByUrl(sectionUrl string) url.Values {
	q := make(url.Values)

	switch sectionUrl {
	case "/gog/owned":
		q.Set(vangogh_integration.GogIsAccountProductProperty, vangogh_integration.TrueValue)
		q.Set(vangogh_integration.UrlSortParameter, vangogh_integration.GogAccountProductOrderProperty)
	case "/gog/sale":
		q.Set(vangogh_integration.GogOwnedProperty, vangogh_integration.FalseValue)
		q.Set(vangogh_integration.GogIsDiscountedProperty, vangogh_integration.TrueValue)
		q.Set(vangogh_integration.UrlSortParameter, vangogh_integration.GogDiscountPercentageProperty)
		q.Set(vangogh_integration.UrlDescendingParameter, vangogh_integration.TrueValue)
	case "/gog/wishlist":
		q.Set(vangogh_integration.GogUserWishlistProperty, vangogh_integration.TrueValue)
		q.Set(vangogh_integration.UrlSortParameter, vangogh_integration.GogReleaseDateProperty)
		q.Set(vangogh_integration.UrlDescendingParameter, vangogh_integration.TrueValue)
	case "/gog/catalog":
		q.Set(vangogh_integration.UrlSortParameter, vangogh_integration.GogReleaseDateProperty)
		q.Set(vangogh_integration.UrlDescendingParameter, vangogh_integration.TrueValue)
	}

	return q
}
