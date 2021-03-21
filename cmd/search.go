package cmd

import (
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/froth"
	"strings"
)

var queryProperties = map[string][]string{
	vangogh_properties.AllTextProperties:    vangogh_properties.AllText(),
	vangogh_properties.AllImageIdProperties: vangogh_properties.AllImageId(),
	vangogh_properties.TitleProperty:        {vangogh_properties.TitleProperty},
	vangogh_properties.DeveloperProperty:    {vangogh_properties.DeveloperProperty},
	vangogh_properties.PublisherProperty:    {vangogh_properties.PublisherProperty},
}

func Search(query map[string]string) error {

	properties := []string{vangogh_properties.TitleProperty}
	for prop, _ := range query {
		properties = mergeProperties(properties, queryProperties[prop])
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	matchingIdsProps := make(map[string][]string, 0)

	for prop, term := range query {
		mergeMatchingIdsProps(
			matchingIdsProps,
			matchingIds(term, queryProperties[prop], propExtracts))
	}

	for id, props := range matchingIdsProps {
		// passing term of the -image-id query to allow filtering screenshots values
		printInfo(id, query[vangogh_properties.AllImageIdProperties], props, propExtracts, nil)
	}

	return nil
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
			matchingIdsProps[id] = append(matchingIdsProps[id], props...)
		}
	}
}

func matchingIds(term string, properties []string, propExtracts map[string]*froth.Stash) map[string][]string {
	ids := make(map[string][]string, 0)
	term = strings.ToLower(term)
	for prop, extracts := range propExtracts {
		for _, property := range properties {
			if prop != property {
				continue
			}

			for _, id := range extracts.All() {
				val, ok := extracts.Get(id)
				if !ok || val == "" {
					continue
				}

				if strings.Contains(strings.ToLower(val), term) {
					ids[id] = append(ids[id], prop)
				}
			}
		}
	}
	return ids
}
