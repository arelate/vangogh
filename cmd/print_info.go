package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"strings"
)

func shouldSkip(value string, filterValues []string) bool {
	value = strings.ToLower(value)
	for _, fv := range filterValues {
		if strings.Contains(value, fv) {
			return false
		}
	}
	// this makes sure we don't filter values if there is no filter
	return len(filterValues) > 0
}

func printInfo(
	id string,
	isNew bool,
	propertyFilter map[string][]string,
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
		filterValues := propertyFilter[prop]

		if len(values) > 1 && vangogh_properties.JoinPreferred(prop) {
			joinedValue := strings.Join(values, ",")
			if shouldSkip(joinedValue, filterValues) {
				continue
			}
			fmt.Printf(" %s:%s\n", prop, joinedValue)
			continue
		}

		for _, val := range values {
			if shouldSkip(val, filterValues) {
				continue
			}
			fmt.Printf(" %s:%s\n", prop, val)
		}
	}
}
