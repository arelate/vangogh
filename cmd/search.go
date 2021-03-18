package cmd

import (
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/froth"
	"strings"
)

func Search(text, title, developer, publisher, imageId string) error {

	properties := []string{vangogh_properties.TitleProperty}
	if text != "" {
		properties = append(properties, vangogh_properties.AllTextProperties()...)
	}
	if imageId != "" {
		properties = append(properties, vangogh_properties.AllImageIdProperties()...)
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	matchingIdsProps := make(map[string][]string, 0)

	if text != "" {
		matchingIdsProps = matchingIds(text, vangogh_properties.AllTextProperties(), propExtracts)
	}

	if imageId != "" {
		imageMatchingIdsProps := matchingIds(imageId, vangogh_properties.AllImageIdProperties(), propExtracts)
		for id, props := range imageMatchingIdsProps {
			if _, ok := matchingIdsProps[id]; !ok {
				matchingIdsProps[id] = props
			} else {
				matchingIdsProps[id] = append(matchingIdsProps[id], props...)
			}
		}
	}

	for id, props := range matchingIdsProps {
		printInfo(id, props, propExtracts)
	}

	return nil
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
