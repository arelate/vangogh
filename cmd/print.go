package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_sets"
	"sort"
	"strings"
)

//TODO: add sort property
func PrintGroups(
	groupIds map[string][]string) error {

	groups := make([]string, 0, len(groupIds))
	for grp, _ := range groupIds {
		groups = append(groups, grp)
	}

	sort.Strings(groups)

	exl, err := vangogh_extracts.NewList(vangogh_properties.TitleProperty)
	if err != nil {
		return err
	}

	for _, grp := range groups {
		if len(groupIds[grp]) == 0 {
			continue
		}
		sorted := vangogh_sets.
			IdSetWith(groupIds[grp]...).
			Sort(exl, vangogh_properties.TitleProperty, false)

		fmt.Printf(" %s:\n", grp)

		Print(sorted, nil, []string{vangogh_properties.TitleProperty}, exl)
	}

	return nil
}

// TODO: add sortBy property
func Print(
	ids []string,
	propertyFilter map[string][]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) error {
	if exl == nil {
		var err error
		exl, err = vangogh_extracts.NewList(properties...)
		if err != nil {
			return err
		}
	}

	for _, id := range ids {
		if err := printInfo(id, propertyFilter, properties, exl); err != nil {
			return err
		}
	}

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
