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

func Distill(pt vangogh_types.ProductType, mt gog_types.Media, properties []string) error {
	for _, property := range properties {
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

	vrProd, err := vangogh_values.NewReader(pt, mt)
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

	for _, id := range vrProd.All() {
		if val, ok := propDistStash.Get(id); ok && val != "" {
			continue
		}

		prodProp, ok := prodPropStash.Get(id)
		if !ok || prodProp == "" {
			log.Fatalf("vangogh: stash doesn't contain property %s for %s (%s) %s", property, pt, mt, id)
		}

		if err := propDistStash.Set(id, prodProp); err != nil {
			return err
		}
	}

	return nil
}
