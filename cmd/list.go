package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/output"
	"time"
)

//List prints products of a certain type and media.
//Can be filtered to products that were created or modified since a certain time.
//Provided properties will be printed for each product (if supported) in addition to default ID, Title.
func List(
	idSet gost.StrSet,
	modifiedAfter int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	properties []string) error {

	if !vangogh_products.Valid(pt) {
		return fmt.Errorf("can't list invalid product type %s", pt)
	}
	if !gog_media.Valid(mt) {
		return fmt.Errorf("can't list invalid media %s", mt)
	}

	propSet := gost.NewStrSetWith(properties...)

	//if no properties have been provided - print ID, Title
	if propSet.Len() == 0 {
		propSet.Add(
			vangogh_properties.IdProperty,
			vangogh_properties.TitleProperty)
	}

	//if Title property has not been provided - add it.
	//we'll always print the title.
	//same goes for sort-by property
	propSet.Add(vangogh_properties.TitleProperty)

	//rules for collecting IDs to print:
	//1. start with user provided IDs
	//2. if createdAfter has been provided - add products created since that time
	//3. if modifiedAfter has been provided - add products modified (not by creation!) since that time
	//4. if no IDs have been collected and the request have not provided createdAfter or modifiedAfter:
	// add all product IDs

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	if modifiedAfter > 0 {
		idSet.Add(vr.ModifiedAfter(modifiedAfter, false)...)
		if idSet.Len() == 0 {
			fmt.Printf("no new or updated %s (%s) since %v\n", pt, mt, time.Unix(modifiedAfter, 0).Format(time.Kitchen))
		}
	}

	if idSet.Len() == 0 &&
		modifiedAfter == 0 {
		idSet.Add(vr.All()...)
	}

	return output.Items(
		idSet.All(),
		nil,
		vangogh_properties.Supported(pt, properties),
		nil)
}
