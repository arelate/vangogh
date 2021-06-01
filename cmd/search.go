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
	properties := map[string]bool{vangogh_properties.TitleProperty: true}
	for qp, _ := range query {
		properties[qp] = true
	}

	exl, err := vangogh_extracts.NewListFromMap(properties)
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

	for _, id := range results {
		if err := printInfo(
			id,
			propertyFilter,
			//similarly for propertyFilter (see comment above) - expand all properties to display
			vangogh_properties.ExpandAll(properties),
			exl); err != nil {
			return err
		}
	}

	if len(results) == 0 {
		fmt.Println("no products found")
	}

	return nil
}
