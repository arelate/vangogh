package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/output"
)

func Search(query map[string][]string) error {

	//prepare a list of all properties to load extracts for and
	//always start with a `title` property since it is printed for all matched item
	//(even if the match is for another property)
	propSet := gost.NewStrSetWith(vangogh_properties.TitleProperty)
	for qp, _ := range query {
		propSet.Add(qp)
	}

	exl, err := vangogh_extracts.NewList(propSet.All()...)
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

	if len(results) == 0 {
		fmt.Println("no products found")
		return nil
	}

	return output.Items(
		results,
		propertyFilter,
		//similarly for propertyFilter (see comment above) - expand all properties to display
		vangogh_properties.ExpandAll(propSet.All()),
		exl)
}
