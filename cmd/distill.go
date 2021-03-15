package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/froth"
	"log"
	"strings"
)

func Distill(pt vangogh_types.ProductType, mt gog_types.Media) error {
	for _, property := range vangogh_properties.AllStashedProperties() {
		if err := distillProperty(pt, mt, property); err != nil {
			return err
		}
	}

	return nil
}

func distillProperty(pt vangogh_types.ProductType, mt gog_types.Media, property string) error {
	fmt.Printf("distilling %s (%s) %s\n", pt, mt, property)

	distStashUrl := vangogh_urls.DistilledStashUrl()

	prodStashUrl, err := vangogh_urls.ProductTypeStashUrl(pt, mt)
	if err != nil {
		return err
	}

	if !vangogh_properties.SupportsProperty(pt, property) {
		log.Printf("vangogh: product type %s doesn't support %s property\n", pt, property)
		return nil
	}

	prodPropStash, err := froth.NewStash(prodStashUrl, property)
	if err != nil {
		return err
	}
	propDistStash, err := froth.NewStash(distStashUrl, property)
	if err != nil {
		return err
	}

	distStashValues := make(map[string]string, len(prodPropStash.All()))

	for _, id := range prodPropStash.All() {
		distVal, distOk := propDistStash.Get(id)

		prodPropVal, prodOk := prodPropStash.Get(id)
		if !prodOk {
			log.Fatalf("vangogh: stash doesn't contain property %s for %s (%s) %s", property, pt, mt, id)
		}

		if distOk &&
			prodOk &&
			len(distVal) == len(prodPropVal) &&
			strings.Contains(distVal, prodPropVal) {
			// keeping same length or longer existing values in the stash
			continue
		}

		distStashValues[id] = prodPropVal
	}

	return propDistStash.SetMany(distStashValues)
}
