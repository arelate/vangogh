package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
	"strings"
)

func Info(ids []string, mt gog_types.Media, images bool) error {
	var err error
	productTypeReaders := make(map[vangogh_types.ProductType]*vangogh_values.ValueReader)
	for _, pt := range vangogh_types.AllLocalProductTypes() {
		productTypeReaders[pt], err = vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}
	}

	properties := vangogh_properties.AllText()
	if images {
		properties = append(properties, vangogh_properties.AllImageId()...)
	}

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	for _, id := range ids {
		printInfo(id, "", properties, propExtracts, productTypeReaders)
	}
	return nil
}

func printInfo(
	id string,
	value string,
	properties []string,
	propExtracts map[string]*froth.Stash,
	productTypeReaders map[vangogh_types.ProductType]*vangogh_values.ValueReader) {

	titleExtracts := propExtracts[vangogh_properties.TitleProperty]
	title, ok := titleExtracts.Get(id)
	if ok {
		fmt.Printf("%s \"%s\"\n", id, title)
	} else {
		fmt.Printf("no information for id %s\n", id)
	}

	if productTypeReaders != nil {
		ptStrings := make([]string, 0)
		for pt := range productTypeReaders {
			if productTypeReaders[pt].Contains(id) {
				ptStrings = append(ptStrings, pt.String())
			}
		}
		if len(ptStrings) > 0 {
			fmt.Printf(" types:%s\n", strings.Join(ptStrings, ","))
		}
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
				if value != "" && !strings.Contains(scr, value) {
					continue
				}
				fmt.Printf(" %s:\"%s\"\n", prop, scr)
			}
		} else {
			fmt.Printf(" %s:\"%s\"\n", prop, val)
		}
	}
}
