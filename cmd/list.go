package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_values"
	"strings"
)

func List(ids []string, pt vangogh_types.ProductType, mt gog_types.Media, properties ...string) error {
	if !vangogh_types.ValidProductType(pt) {
		return fmt.Errorf("vangogh: can't list invalid product type %s", pt)
	}
	if !gog_types.ValidMedia(mt) {
		return fmt.Errorf("vangogh: can't list invalid media %s", mt)
	}

	if len(properties) == 0 {
		properties = []string{
			vangogh_properties.IdProperty,
			vangogh_properties.TitleProperty}
	}

	propStashes, err := vangogh_properties.PropStashes(properties)
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

	sep := " "
	if len(properties) > 2 {
		sep = ","
	}

	output := make([]string, len(properties))
	for _, id := range ids {

		for i, prop := range properties {
			if prop == vangogh_properties.IdProperty {
				output[i] = id
				continue
			}
			if propStashes[prop] == nil {
				output[i] = ""
			}
			val, ok := propStashes[prop].Get(id)
			if !ok || val == "" {
				output[i] = ""
			}

			output[i] = fmt.Sprintf("\"%s\"", val)
		}

		fmt.Println(strings.Join(output, sep))
	}

	return nil
}
