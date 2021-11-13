package cli_api

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/expand"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
)

func SearchHandler(u *url.URL) error {
	query := make(map[string][]string)
	for _, prop := range vangogh_properties.Searchable() {
		if values := url_helpers.Values(u, prop); len(values) > 0 {
			query[prop] = values
		}
	}
	return Search(query)
}

func Search(query map[string][]string) error {

	sa := nod.Begin("searching...")
	defer sa.End()

	//prepare a list of all properties to load extracts for and
	//always start with a `title` property since it is printed for all matched item
	//(even if the match is for another property)
	propSet := gost.NewStrSetWith(vangogh_properties.TitleProperty)
	for qp, _ := range query {
		propSet.Add(qp)
	}

	exl, err := vangogh_extracts.NewList(propSet.All()...)
	if err != nil {
		return sa.EndWithError(err)
	}

	results := exl.Search(query, true)

	//expand query properties for use in printInfo filter
	//since it's not aware of collapsed/expanded properties concept
	propertyFilter := make(map[string][]string, 0)

	for prop, terms := range query {
		if vangogh_properties.IsCollapsed(prop) {
			for _, ep := range vangogh_properties.Expand(prop) {
				propertyFilter[ep] = terms
			}
		} else {
			propertyFilter[prop] = terms
		}
	}

	if len(results) == 0 {
		sa.EndWithResult("no products found")
		return nil
	}

	itp, err := expand.IdsToPropertyLists(
		"found products:",
		results,
		propertyFilter,
		//similarly for propertyFilter (see comment above) - expand all properties to display
		vangogh_properties.ExpandAll(propSet.All()),
		exl)

	if err != nil {
		return sa.EndWithError(err)
	}

	sa.EndWithSummary(itp)

	return nil
}
