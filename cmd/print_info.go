package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
	"sort"
	"strings"
)

func printInfo(
	id string,
	highlightValue string,
	properties []string,
	propExtracts map[string]*froth.Stash,
	productTypeReaders map[vangogh_products.ProductType]*vangogh_values.ValueReader) {

	titleExtracts := propExtracts[vangogh_properties.TitleProperty]
	title, ok := titleExtracts.Get(id)
	if !ok {
		fmt.Printf("no title for id %s\n", id)
		return
	}

	fmt.Println(id, title)

	if productTypeReaders != nil {
		ptStrings := make([]string, 0)
		for pt := range productTypeReaders {
			if productTypeReaders[pt].Contains(id) {
				ptStrings = append(ptStrings, pt.String())
			}
		}
		sort.Strings(ptStrings)
		if len(ptStrings) > 0 {
			fmt.Printf(" types:%s\n", strings.Join(ptStrings, ","))
		}
	}

	for _, prop := range properties {
		if prop == vangogh_properties.TitleProperty {
			continue
		}
		values, ok := propExtracts[prop].GetAll(id)
		if !ok || len(values) == 0 {
			continue
		}
		for _, val := range values {
			if highlightValue != "" && !strings.Contains(val, highlightValue) {
				continue
			}
			fmt.Printf(" %s:%v\n", prop, val)
		}
	}
}
