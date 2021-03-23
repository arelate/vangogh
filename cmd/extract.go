package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
	"log"
)

func Extract(mt gog_media.Media, properties []string) error {

	if len(properties) == 0 {
		properties = vangogh_properties.AllExtracted()
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	for _, pt := range vangogh_products.AllLocal() {

		log.Printf("extract %s", pt)

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		missingPropExtracts := make(map[string]map[string]interface{}, 0)

		for _, id := range vr.All() {

			missingProps := missingProperties(pt, propExtracts, id)
			if len(missingProps) == 0 {
				continue
			}

			propValues, err := vangogh_properties.GetProperties(id, vr, missingProps)
			if err != nil {
				return err
			}

			for prop, value := range propValues {
				if _, ok := missingPropExtracts[prop]; !ok {
					missingPropExtracts[prop] = make(map[string]interface{}, 0)
				}
				missingPropExtracts[prop][id] = value
			}
		}

		for prop, extracts := range missingPropExtracts {
			if err := propExtracts[prop].SetMany(extracts); err != nil {
				return err
			}
		}
	}

	return nil
}

func missingProperties(pt vangogh_products.ProductType, propStashes map[string]*froth.Stash, id string) []string {
	missingProps := make([]string, 0)
	for prop, stash := range propStashes {
		if _, ok := stash.Get(id); !ok && vangogh_properties.SupportsProperty(pt, prop) {
			missingProps = append(missingProps, prop)
		}
	}
	return missingProps
}
