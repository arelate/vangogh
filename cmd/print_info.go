package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"strings"
)

func printInfo(
	id string,
	isNew bool,
	filterValues map[string]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) {

	title, ok := exl.Get(vangogh_properties.TitleProperty, id)
	if !ok {
		fmt.Printf("product %s not found\n", id)
		return
	}

	if isNew {
		title = fmt.Sprintf("[NEW] %s", title)
	}

	fmt.Println(id, title)

	for _, prop := range properties {
		if prop == vangogh_properties.IdProperty ||
			prop == vangogh_properties.TitleProperty {
			continue
		}
		values, ok := exl.GetAll(prop, id)
		if !ok || len(values) == 0 {
			continue
		}
		if len(values) > 1 && vangogh_properties.JoinPreferred(prop) {
			fmt.Printf(" %s:%s\n", prop, strings.Join(values, ","))
			continue
		}
		//TODO: redo this to filter all properties based on the filter
		for _, val := range values {
			highlight := filterValues[prop]
			if highlight != "" && !strings.Contains(strings.ToLower(val), highlight) {
				continue
			}
			fmt.Printf(" %s:%s\n", prop, val)
		}
	}
}
