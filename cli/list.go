package cli

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
	"time"
)

func ListHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	return List(
		ids,
		since,
		vangogh_integration.ProductTypeFromUrl(u),
		vangogh_integration.PropertiesFromUrl(u))
}

// List prints products of a certain product type.
// Can be filtered to products that were created or modified since a certain time.
// Provided properties will be printed for each product (if supported) in addition to default ID, Title.
func List(
	ids []string,
	modifiedSince int64,
	pt vangogh_integration.ProductType,
	properties []string) error {

	la := nod.Begin("listing %s...", pt)
	defer la.End()

	if !vangogh_integration.IsValidProductType(pt) {
		return la.EndWithError(fmt.Errorf("can't list invalid product type %s", pt))
	}

	propSet := make(map[string]bool)
	for _, p := range properties {
		propSet[p] = true
	}

	//if no properties have been provided - print ID, Title
	if len(propSet) == 0 {
		propSet[vangogh_integration.IdProperty] = true
		propSet[vangogh_integration.TitleProperty] = true
	}

	//if Title property has not been provided - add it.
	//we'll always print the title.
	//same goes for sort-by property
	propSet[vangogh_integration.TitleProperty] = true

	//rules for collecting IDs to print:
	//1. start with user provided IDs
	//2. if createdAfter has been provided - add products created since that time
	//3. if modifiedAfter has been provided - add products modified (not by creation!) since that time
	//4. if no IDs have been collected and the request have not provided createdAfter or modifiedAfter:
	// add all product IDs

	vr, err := vangogh_integration.NewProductReader(pt)
	if err != nil {
		return la.EndWithError(err)
	}

	if modifiedSince > 0 {
		for id := range vr.CreatedOrUpdatedAfter(modifiedSince) {
			ids = append(ids, id)
		}

		if len(ids) == 0 {
			la.EndWithResult("no new or updated %s (%s) since %v\n", pt, time.Unix(modifiedSince, 0).Format(time.Kitchen))
		}
	}

	if len(ids) == 0 &&
		modifiedSince == 0 {
		for id := range vr.Keys() {
			ids = append(ids, id)
		}
	}

	itp, err := vangogh_integration.PropertyListsFromIdSet(
		ids,
		nil,
		vangogh_integration.SupportedPropertiesOnly(pt, maps.Keys(propSet)),
		nil)

	if err != nil {
		return la.EndWithError(err)
	}

	la.EndWithSummary("", itp)

	return nil
}
