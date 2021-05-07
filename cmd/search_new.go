package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
)

func SearchNew(query map[string]string) error {

	properties := []string{vangogh_properties.TitleProperty}
	for prop, _ := range query {
		properties = append(properties, prop)
	}

	exl, err := vangogh_extracts.NewList(properties...)
	if err != nil {
		return err
	}

	results := exl.Search(query, true)

	for _, id := range results {
		printInfo(id, false, query, properties, exl)
	}

	return nil
}
