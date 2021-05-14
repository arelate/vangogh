package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"strings"
)

func stringsTrimSpace(stringsWithSpace []string) []string {
	trimmedStrings := make([]string, 0, len(stringsWithSpace))
	for _, str := range stringsWithSpace {
		trimmedStrings = append(trimmedStrings, strings.TrimSpace(str))
	}
	return trimmedStrings
}

func extractTagNames(mt gog_media.Media) error {
	fmt.Println("extract tag names")
	vrAccountPage, err := vangogh_values.NewReader(vangogh_products.AccountPage, mt)
	if err != nil {
		return err
	}

	const fpId = "1"
	if !vrAccountPage.Contains(fpId) {
		return fmt.Errorf("vangogh: %s doesn't contain page %s", vangogh_products.AccountPage, fpId)
	}

	firstPage, err := vrAccountPage.AccountPage(fpId)
	if err != nil {
		return err
	}

	exl, err := vangogh_extracts.NewList(vangogh_properties.TagNameProperty)
	if err != nil {
		return err
	}

	tagIdNames := make(map[string][]string, 0)

	for _, tag := range firstPage.Tags {
		tagIdNames[tag.Id] = []string{tag.Name}
	}

	return exl.AddMany(vangogh_properties.TagNameProperty, tagIdNames)
}

func Extract(modifiedAfter int64, mt gog_media.Media, properties []string) error {

	if len(properties) == 0 {
		properties = vangogh_properties.Extracted()
	}

	typeExtracts, err := vangogh_extracts.NewList(vangogh_properties.TypesProperty)
	if err != nil {
		return err
	}

	exl, err := vangogh_extracts.NewList(properties...)
	if err != nil {
		return err
	}

	idsTypes := make(map[string][]string)

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		missingProps := vangogh_properties.Supported(pt, properties)

		missingPropExtracts := make(map[string]map[string][]string, 0)

		var modifiedIds []string
		if modifiedAfter > 0 {
			modifiedIds = vr.ModifiedAfter(modifiedAfter, false)
		} else {
			modifiedIds = vr.All()
		}

		if len(modifiedIds) == 0 {
			continue
		}

		fmt.Printf("extract %s\n", pt)

		for _, id := range modifiedIds {

			if idsTypes[id] == nil {
				idsTypes[id] = make([]string, 0)
			}

			idsTypes[id] = append(idsTypes[id], pt.String())

			if len(missingProps) == 0 {
				continue
			}

			propValues, err := vangogh_properties.GetProperties(id, vr, missingProps)
			if err != nil {
				return err
			}

			for prop, values := range propValues {
				if _, ok := missingPropExtracts[prop]; !ok {
					missingPropExtracts[prop] = make(map[string][]string, 0)
				}
				missingPropExtracts[prop][id] = stringsTrimSpace(values)
			}
		}

		for prop, extracts := range missingPropExtracts {
			if err := exl.AddMany(prop, extracts); err != nil {
				return err
			}
		}
	}

	//tag-names are extracted separately from other types,
	//given it's most convenient to extract from account-pages
	if err := extractTagNames(mt); err != nil {
		return err
	}

	return typeExtracts.AddMany(vangogh_properties.TypesProperty, idsTypes)
}
