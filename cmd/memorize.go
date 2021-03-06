package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_values"
)

func Memorize(pt vangogh_types.ProductType, mt gog_types.Media, properties []string) error {
	for _, property := range properties {
		if err := memorizeProperty(pt, mt, property); err != nil {
			return err
		}
	}
	return nil
}

func memorizeProperty(pt vangogh_types.ProductType, mt gog_types.Media, property string) error {
	if !vangogh_types.ValidProductType(pt) {
		return fmt.Errorf("vangogh: invalid product type %s", pt)
	}
	if !gog_types.ValidMedia(mt) {
		return fmt.Errorf("vangogh: invalid media %s", mt)
	}

	fmt.Printf("memorizing %s (%s) %s\n", pt, mt, property)

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	var propertyGetter func(string, vangogh_types.ProductType, gog_types.Media) (string, error)

	switch property {
	case vangogh_properties.TitleProperty:
		propertyGetter = vangogh_properties.GetTitle
	default:
		return fmt.Errorf("vangogh: unsupported property %s", property)
	}

	for _, id := range vr.All() {
		prop, err := propertyGetter(id, pt, mt)
		if err != nil {
			return err
		}

		fmt.Println(id, prop)
	}

	return nil
}
