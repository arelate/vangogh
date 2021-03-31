package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"log"
	"math"
	"time"
)

func List(ids []string, createdAfter, modifiedAfter int64, pt vangogh_products.ProductType, mt gog_media.Media, properties ...string) error {
	if !vangogh_products.Valid(pt) {
		return fmt.Errorf("can't list invalid product type %s", pt)
	}
	if !gog_media.Valid(mt) {
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

	if createdAfter > 0 {
		ids = vr.CreatedAfter(createdAfter)
		if len(ids) == 0 {
			hours := math.Round(time.Now().Sub(time.Unix(createdAfter, 0)).Hours())
			log.Printf("no %s (%s) created in the last %v hour(s)", pt, mt, hours)
		}
	}

	if modifiedAfter > 0 {
		ids = vr.ModifiedAfter(modifiedAfter)
		if len(ids) == 0 {
			hours := math.Round(time.Now().Sub(time.Unix(modifiedAfter, 0)).Hours())
			log.Printf("no %s (%s) modified in the last %v hour(s)", pt, mt, hours)
		}
	}

	if createdAfter == 0 && modifiedAfter == 0 && len(ids) == 0 {
		ids = vr.All()
	}

	for _, id := range ids {
		printInfo(id, "", properties, propExtracts, nil)
	}

	return nil
}
