package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"log"
	"time"
)

//List prints products of a certain type and media.
//Can be filtered to products that were created or modified since a certain time.
//Provided properties will be printed for each product (if supported) in addition to default ID, Title.
func List(
	ids []string,
	createdSince, modifiedSince int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	properties ...string) error {

	if !vangogh_products.Valid(pt) {
		return fmt.Errorf("can't list invalid product type %s", pt)
	}
	if !gog_media.Valid(mt) {
		return fmt.Errorf("can't list invalid media %s", mt)
	}

	//if no properties have been provided - print ID, Title
	if len(properties) == 0 {
		properties = []string{
			vangogh_properties.IdProperty,
			vangogh_properties.TitleProperty}
	}

	//if Title property has not been provided - add it first.
	//we'll always print the title
	if !stringsContain(properties, vangogh_properties.TitleProperty) {
		properties = append([]string{vangogh_properties.TitleProperty}, properties...)
	}

	//rules for collecting IDs to print:
	//1. start with user provided IDs
	//2. if createdSince has been provided - add products created since that time
	//3. if modifiedSince has been provided - add products modified (not by creation!) since that time
	//4. if no IDs have been collected and the request have not provided createdSince or modifiedSince:
	// add all product IDs

	if ids == nil {
		ids = make([]string, 0)
	}

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	if createdSince > 0 {
		ids = append(ids, vr.CreatedAfter(createdSince)...)
		if len(ids) == 0 {
			log.Printf("no %s (%s) created since %v", pt, mt, time.Unix(createdSince, 0))
		}
	}

	if modifiedSince > 0 {
		ids = append(ids, vr.ModifiedAfter(modifiedSince)...)
		if len(ids) == 0 {
			log.Printf("no %s (%s) modified since %v", pt, mt, time.Unix(modifiedSince, 0))
		}
	}

	if len(ids) == 0 &&
		createdSince == 0 &&
		modifiedSince == 0 {
		ids = vr.All()
	}

	//only attempt to print supported properties by that product type
	supportedProperties := make([]string, 0, len(properties))
	for _, prop := range properties {
		if vangogh_properties.SupportsProperty(pt, prop) {
			supportedProperties = append(supportedProperties, prop)
		}
	}

	//load properties extract that'll be used for printing
	propExtracts, err := vangogh_properties.PropExtracts(properties)

	//use common printInfo func to display product information by ID
	for _, id := range ids {
		printInfo(id, "", supportedProperties, propExtracts, nil)
	}

	return nil
}
