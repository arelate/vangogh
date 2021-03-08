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
	"strings"
)

func List(ids []string, pt vangogh_types.ProductType, mt gog_types.Media, properties ...string) error {

	if len(properties) == 0 {
		properties = []string{vangogh_properties.IdProperty, vangogh_properties.TitleProperty}
	}

	distStashUrl := vangogh_urls.DistilledStashUrl()

	propStashes := make(map[string]*froth.Stash, len(properties))
	for _, prop := range properties {
		if prop == vangogh_properties.IdProperty {
			continue
		}
		if !vangogh_properties.ValidProperty(prop) {
			log.Printf("vangogh: invalid property %s", prop)
			continue
		}
		stash, err := froth.NewStash(distStashUrl, prop)
		if err != nil {
			return err
		}
		propStashes[prop] = stash
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
