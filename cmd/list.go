package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_values"
)

func List(ids []string, pt vangogh_types.ProductType, mt gog_types.Media, properties ...string) error {
	if !vangogh_types.ValidProductType(pt) {
		return fmt.Errorf("can't list invalid product type %s", pt)
	}
	if !gog_types.ValidMedia(mt) {
		return fmt.Errorf("can't list invalid media %s", mt)
	}

	if len(properties) == 0 {
		properties = []string{
			vangogh_properties.IdProperty,
			vangogh_properties.TitleProperty}
	}

	containsTitle := false
	for _, prop := range properties {
		if prop == vangogh_properties.TitleProperty {
			containsTitle = true
			break
		}
	}

	if !containsTitle {
		properties = append([]string{vangogh_properties.TitleProperty}, properties...)
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		ids = vr.All()
	}

	for _, id := range ids {
		printInfo(id, "", properties, propExtracts, nil)
	}

	return nil
}
