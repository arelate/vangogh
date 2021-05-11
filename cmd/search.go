package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
)

func Search(query map[string][]string) error {

	//prepare a list of all properties to load extracts for and
	//always start with a `title` property since it is printed for all matched item
	//(even if the match is for another property)
	properties := []string{vangogh_properties.TitleProperty}
	for prop, _ := range query {
		properties = append(properties, prop)
	}

	exl, err := vangogh_extracts.NewList(properties...)
	if err != nil {
		return err
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

	if len(results) > 0 {
		fmt.Printf("found %d products:\n", len(results))
	} else {
		fmt.Println("zero products found")
	}

	for _, id := range results {
		printInfo(
			id,
			false,
			propertyFilter,
			//similarly for propertyFilter (see comment above) - expand all properties to display
			vangogh_properties.ExpandAll(properties),
			exl)
	}

	return nil
}
