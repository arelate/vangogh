package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
)

func Extract(modifiedAfter int64, mt gog_media.Media, properties []string) error {

	if len(properties) == 0 {
		properties = vangogh_properties.Extracted()
	}

	typeExtracts, err := froth.NewStash(vangogh_urls.ExtractsDir(), vangogh_properties.TypesProperty)
	if err != nil {
		return err
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
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
			modifiedIds = vr.ModifiedAfter(modifiedAfter)
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
				missingPropExtracts[prop][id] = values
			}
		}

		for prop, extracts := range missingPropExtracts {
			if err := propExtracts[prop].AddMany(extracts); err != nil {
				return err
			}
		}
	}

	return typeExtracts.AddMany(idsTypes)
}
