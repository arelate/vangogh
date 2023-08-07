package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
	"strings"
)

func SearchHandler(u *url.URL) error {
	query := make(map[string][]string)

	for _, prop := range vangogh_local_data.ReduxProperties() {
		if values := vangogh_local_data.ValuesFromUrl(u, prop); len(values) > 0 {
			query[prop] = values
		}
	}

	return Search(query)
}

// Search using provided property: values query
func Search(query map[string][]string) error {

	sa := nod.Begin("searching...")
	defer sa.End()

	//prepare a list of all properties to load redux for and
	//always start with a `title` property since it is printed for all matched item
	//(even if the match is for another property)
	propSet := map[string]bool{vangogh_local_data.TitleProperty: true}
	for qp := range query {
		propSet[qp] = true
	}

	rxa, err := vangogh_local_data.ConnectReduxAssets(maps.Keys(propSet)...)
	if err != nil {
		return sa.EndWithError(err)
	}

	results := rxa.Match(query, true, true)

	//expand query properties for use in printInfo filter
	//since it's not aware of collapsed/expanded properties concept
	propertyFilter := make(map[string][]string, 0)

	for prop, terms := range query {
		lowerCaseTerms := make([]string, 0, len(terms))
		for _, t := range terms {
			lowerCaseTerms = append(lowerCaseTerms, strings.ToLower(t))
		}
		//if vangogh_local_data.IsPropertyAggregate(prop) {
		//	for _, ep := range vangogh_local_data.DetailAggregateProperty(prop) {
		//		propertyFilter[ep] = lowerCaseTerms
		//	}
		//} else {
		propertyFilter[prop] = lowerCaseTerms
		//}
	}

	if len(results) == 0 {
		sa.EndWithResult("no products found")
		return nil
	}

	//similarly for propertyFilter (see comment above) - expand all properties to display
	//expandedPropsMap := vangogh_local_data.DetailAllAggregateProperties(maps.Keys(propSet)...)
	//expandedProps := make([]string, 0, len(expandedPropsMap))
	//for p := range expandedPropsMap {
	//	expandedProps = append(expandedProps, p)
	//}

	itp, err := vangogh_local_data.PropertyListsFromIdSet(
		results,
		propertyFilter,
		maps.Keys(propSet),
		rxa)

	if err != nil {
		return sa.EndWithError(err)
	}

	sa.EndWithSummary("found products:", itp)

	return nil
}
