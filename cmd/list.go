package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"time"
)

//List prints products of a certain type and media.
//Can be filtered to products that were created or modified since a certain time.
//Provided properties will be printed for each product (if supported) in addition to default ID, Title.
func List(
	ids []string,
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

	propSet := gost.StrSetWith(properties...)

	//if no properties have been provided - print ID, Title
	if propSet.Len() == 0 {
		propSet.Add(
			vangogh_properties.IdProperty,
			vangogh_properties.TitleProperty)
	}

	//if Title property has not been provided - add it first.
	//we'll always print the title
	if !propSet.Has(vangogh_properties.TitleProperty) {
		propSet.Add(vangogh_properties.TitleProperty)
	}

	//rules for collecting IDs to print:
	//1. start with user provided IDs
	//2. if createdAfter has been provided - add products created since that time
	//3. if modifiedAfter has been provided - add products modified (not by creation!) since that time
	//4. if no IDs have been collected and the request have not provided createdAfter or modifiedAfter:
	// add all product IDs

	idSet := gost.StrSetWith(ids...)

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	if modifiedAfter > 0 {
		for _, mId := range vr.ModifiedAfter(modifiedAfter, false) {
			idSet.Add(mId)
		}
		if idSet.Len() == 0 {
			fmt.Printf("no new or updated %s (%s) since %v\n", pt, mt, time.Unix(modifiedAfter, 0).Format(time.Kitchen))
		}
	}

	if len(ids) == 0 &&
		modifiedAfter == 0 {
		for _, id := range vr.All() {
			idSet.Add(id)
		}
	}

	//load properties extract that will be used for printing
	exl, err := vangogh_extracts.NewList(propSet.All()...)

	//use common printInfo func to display product information by ID
	for _, id := range idSet.All() {
		if err := printInfo(
			id,
			nil,
			vangogh_properties.Supported(pt, properties),
			exl); err != nil {
			return err
		}
	}

	return nil
}
