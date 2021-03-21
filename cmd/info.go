package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/froth"
	"strings"
)

func Info(ids []string, images bool) error {
	properties := vangogh_properties.AllText()
	if images {
		properties = append(properties, vangogh_properties.AllImageId()...)
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	for _, id := range ids {
		printInfo(id, properties, propExtracts)
	}
	return nil
}

func printInfo(id string, properties []string, propExtracts map[string]*froth.Stash) {

	titleExtracts := propExtracts[vangogh_properties.TitleProperty]
	title, ok := titleExtracts.Get(id)
	if ok {
		fmt.Printf("%s \"%s\"\n", id, title)
	} else {
		fmt.Printf("no information for id %s\n", id)
	}

	for _, prop := range properties {
		if prop == vangogh_properties.TitleProperty {
			continue
		}
		val, ok := propExtracts[prop].Get(id)
		if !ok || val == "" {
			continue
		}
		if prop == vangogh_properties.ScreenshotsProperty {
			for _, scr := range strings.Split(val, ",") {
				fmt.Printf(" %s:\"%s\"\n", prop, scr)
			}
		} else {
			fmt.Printf(" %s:\"%s\"\n", prop, val)
		}
	}
}
