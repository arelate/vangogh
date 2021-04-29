package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"strings"
)

func Search(query map[string]string) error {

	queryProps := vangogh_properties.Query()

	properties := []string{vangogh_properties.TitleProperty}
	for prop, _ := range query {
		properties = mergeProperties(properties, queryProps[prop])
	}

	exl, err := vangogh_extracts.NewList(properties...)
	if err != nil {
		return err
	}

	matchingIdsProps := make(map[string][]string, 0)

	// attempt to check if "text" property matches a valid id. Id is not extracted, and
	// is present in every extract as a key. We'll use title extracts to confirm valid ID, given
	// we've explicitly added them above.
	potentialId := query[vangogh_properties.TextProperties]
	if potentialId != "" && exl.Contains(vangogh_properties.TextProperties, potentialId) {
		mergeMatchingIdsProps(
			matchingIdsProps,
			map[string][]string{potentialId: {vangogh_properties.IdProperty}})
	}

	scopeIds := exl.All(vangogh_properties.TitleProperty)

	for prop, term := range query {
		matchedId := matchingIds(scopeIds, term, queryProps[prop], exl)
		scopeIds = make([]string, 0, len(matchedId))
		for id, _ := range matchedId {
			scopeIds = append(scopeIds, id)
		}
		mergeMatchingIdsProps(
			matchingIdsProps,
			matchedId)
	}

	matchingIdsProps = scopeMatchingIdsProps(matchingIdsProps, scopeIds)

	for id, matchingProps := range matchingIdsProps {
		printInfo(
			id,
			false,
			highlights(query, matchingProps),
			matchingProps,
			exl)
	}

	return nil
}

func highlights(query map[string]string, matchingProps []string) map[string]string {
	highlights := make(map[string]string, 0)
	for _, prop := range matchingProps {
		val := query[prop]
		if val == "" {
			val = query[vangogh_properties.Shorthand(prop)]
		}
		if val != "" {
			highlights[prop] = strings.ToLower(val)
		}
	}
	return highlights
}

func mergeProperties(properties []string, newProperties []string) []string {
	for _, newProp := range newProperties {
		contains := false
		for _, prop := range properties {
			if prop == newProp {
				contains = true
				break
			}
		}
		if !contains {
			properties = append(properties, newProp)
		}
	}
	return properties
}

func mergeMatchingIdsProps(matchingIdsProps map[string][]string, newIdsProps map[string][]string) {
	for id, props := range newIdsProps {
		if _, ok := matchingIdsProps[id]; !ok {
			matchingIdsProps[id] = props
		} else {
			for _, prop := range props {
				if !stringsContain(matchingIdsProps[id], prop) {
					matchingIdsProps[id] = append(matchingIdsProps[id], prop)
				}
			}
		}
	}
}

func scopeMatchingIdsProps(matchingIdsProps map[string][]string, scopeIds []string) map[string][]string {
	scopedMatchingIdsProps := make(map[string][]string, 0)
	for id, props := range matchingIdsProps {
		if stringsContain(scopeIds, id) {
			scopedMatchingIdsProps[id] = props
		}
	}
	return scopedMatchingIdsProps
}

func matchingIds(
	scopeIds []string,
	term string,
	properties []string,
	exl *vangogh_extracts.ExtractsList) map[string][]string {

	ids := make(map[string][]string, 0)

	term = strings.ToLower(term)

	for _, property := range properties {

		for _, id := range scopeIds {

			values, ok := exl.GetAll(property, id)
			if !ok || len(values) == 0 {
				continue
			}

			for _, val := range values {
				if strings.Contains(strings.ToLower(val), term) {
					ids[id] = append(ids[id], property)
				}
			}
		}
	}
	return ids
}
