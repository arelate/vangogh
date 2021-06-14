package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"sort"
	"strings"
)

func Print(
	ids []string,
	propertyFilter map[string][]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) error {
	return nil
}

func printInfo(
	id string,
	propertyFilter map[string][]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) error {

	if err := exl.AssertSupport(properties...); err != nil {
		return err
	}

	title, ok := exl.Get(vangogh_properties.TitleProperty, id)
	if !ok {
		fmt.Printf("product %s not found\n", id)
		return nil
	}

	fmt.Println(id, title)

	sort.Strings(properties)

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

	return nil
}

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
