package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
	"log"
)

func Stash(pt vangogh_types.ProductType, mt gog_types.Media) error {
	for _, property := range vangogh_properties.AllStashedProperties() {
		if !vangogh_properties.SupportsProperty(pt, property) {
			log.Printf("vangogh: %s (%s) doesn't support property %s\n", pt, mt, property)
			continue
		}

		if err := stashProperty(pt, mt, property); err != nil {
			return err
		}
	}
	return nil
}

func stashProperty(pt vangogh_types.ProductType, mt gog_types.Media, property string) error {
	if !vangogh_types.ValidProductType(pt) {
		return fmt.Errorf("vangogh: invalid product type %s", pt)
	}
	if !gog_types.ValidMedia(mt) {
		return fmt.Errorf("vangogh: invalid media %s", mt)
	}

	fmt.Printf("stashing %s (%s) %s\n", pt, mt, property)

	stashUrl, err := vangogh_urls.ProductTypeStashUrl(pt, mt)
	if err != nil {
		return err
	}

	stash, err := froth.NewStash(stashUrl, property)

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	missing := make([]string, 0)
	for _, id := range vr.All() {
		if val, ok := stash.Get(id); !ok || val == "" {
			missing = append(missing, id)
		}
	}

	if len(missing) == 0 {
		log.Printf("no missing %s (%s) to stash\n", pt, mt)
		return nil
	}

	propertyValues := make(map[string]string, len(missing))

	for _, id := range missing {
		prop, err := vangogh_properties.GetStrProperty(id, pt, mt, property)
		if err != nil {
			return err
		}

		propertyValues[id] = prop
	}

	return stash.SetMany(propertyValues)
}
