package expand

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_sets"
	"github.com/boggydigital/gost"
	"sort"
	"strings"
)

const (
	DefaultSort = vangogh_properties.TitleProperty
	DefaultDesc = false
)

func IdsToPropertyLists(
	heading string,
	ids []string,
	propertyFilter map[string][]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) (map[string][]string, error) {

	propSet := gost.NewStrSetWith(properties...)
	propSet.Add(vangogh_properties.TitleProperty)

	if exl == nil {
		var err error
		exl, err = vangogh_extracts.NewList(propSet.All()...)
		if err != nil {
			return nil, err
		}
	}

	idSet := vangogh_sets.IdSetWith(ids...)

	itps := make(map[string][]string)
	if heading != "" {
		itps[heading] = []string{}
	}

	for _, id := range idSet.Sort(exl, DefaultSort, DefaultDesc) {
		itp, err := item(id, propertyFilter, propSet.All(), exl)
		if err != nil {
			return itps, err
		}
		for idTitle, props := range itp {
			itps[idTitle] = props
		}
	}

	return itps, nil
}

func item(
	id string,
	propertyFilter map[string][]string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) (map[string][]string, error) {

	if err := exl.AssertSupport(properties...); err != nil {
		return nil, err
	}

	title, ok := exl.Get(vangogh_properties.TitleProperty, id)
	if !ok {
		return nil, nil
	}

	itp := make(map[string][]string)
	idTitle := fmt.Sprintf("%s %s", id, title)
	itp[idTitle] = make([]string, 0)

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
			itp[idTitle] = append(itp[idTitle], fmt.Sprintf("%s:%s", prop, joinedValue))
			continue
		}

		for _, val := range values {
			if shouldSkip(val, filterValues) {
				continue
			}
			itp[idTitle] = append(itp[idTitle], fmt.Sprintf("%s:%s", prop, val))
		}
	}

	return itp, nil
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
