package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"strings"
)

func Info(ids []string, mt gog_types.Media, images bool) error {
	properties := vangogh_properties.AllTextProperties()
	if images {
		properties = append(properties, vangogh_properties.AllImageIdProperties()...)
	}
	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	titleExtracts := propExtracts[vangogh_properties.TitleProperty]

	for _, id := range ids {
		title, _ := titleExtracts.Get(id)
		fmt.Printf("%s \"%s\"\n", id, title)

		for _, prop := range properties {
			if prop == vangogh_properties.TitleProperty {
				continue
			}
			val, ok := propExtracts[prop].Get(id)
			if !ok || val == "" {
				continue
			}
			if prop == vangogh_properties.ScreenshotsProperty {
				fmt.Printf(" %s:\n", prop)
				for _, scr := range strings.Split(val, ",") {
					fmt.Printf("  \"%s\"\n", scr)
				}
			} else {
				fmt.Printf(" %s:\"%s\"\n", prop, val)
			}
		}
	}
	return nil
}
