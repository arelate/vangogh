package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"net/url"
	"slices"
	"strings"
)

func SearchHandler(u *url.URL) error {
	query := make(map[string][]string)

	for _, prop := range vangogh_integration.ReduxProperties() {
		if values := vangogh_integration.ValuesFromUrl(u, prop); len(values) > 0 {
			query[prop] = values
		}
	}

	return Search(query)
}

// Search using provided property: values query
func Search(query map[string][]string) error {

	sa := nod.Begin("searching...")
	defer sa.Done()

	//prepare a list of all properties to load redux for and
	//always start with a `title` property since it is printed for all matched item
	//(even if the match is for another property)
	propSet := map[string]bool{vangogh_integration.TitleProperty: true}
	for qp := range query {
		propSet[qp] = true
	}

	properties := slices.Collect(maps.Keys(propSet))

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir, properties...)
	if err != nil {
		return err
	}

	results := rdx.Match(query)

	//expand query properties for use in printInfo filter
	//since it's not aware of collapsed/expanded properties concept
	propertyFilter := make(map[string][]string, 0)

	for prop, terms := range query {
		lowerCaseTerms := make([]string, 0, len(terms))
		for _, t := range terms {
			lowerCaseTerms = append(lowerCaseTerms, strings.ToLower(t))
		}
		//if vangogh_integration.IsPropertyAggregate(prop) {
		//	for _, ep := range vangogh_integration.DetailAggregateProperty(prop) {
		//		propertyFilter[ep] = lowerCaseTerms
		//	}
		//} else {
		propertyFilter[prop] = lowerCaseTerms
		//}
	}

	if results == nil {
		sa.EndWithResult("no products found")
		return nil
	}

	//similarly for propertyFilter (see comment above) - expand all properties to display
	//expandedPropsMap := vangogh_integration.DetailAllAggregateProperties(maps.Keys(propSet)...)
	//expandedProps := make([]string, 0, len(expandedPropsMap))
	//for p := range expandedPropsMap {
	//	expandedProps = append(expandedProps, p)
	//}

	itp, err := vangogh_integration.PropertyListsFromIdSet(
		slices.Collect(results),
		propertyFilter,
		properties,
		rdx)

	if err != nil {
		return err
	}

	sa.EndWithSummary("found products:", itp)

	return nil
}
