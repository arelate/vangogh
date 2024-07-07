package cli

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
	"time"
)

func ListHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return err
	}

	return List(
		idSet,
		since,
		vangogh_local_data.ProductTypeFromUrl(u),
		vangogh_local_data.PropertiesFromUrl(u))
}

// List prints products of a certain product type.
// Can be filtered to products that were created or modified since a certain time.
// Provided properties will be printed for each product (if supported) in addition to default ID, Title.
func List(
	idSet map[string]bool,
	modifiedSince int64,
	pt vangogh_local_data.ProductType,
	properties []string) error {

	la := nod.Begin("listing %s...", pt)
	defer la.End()

	if !vangogh_local_data.IsValidProductType(pt) {
		return la.EndWithError(fmt.Errorf("can't list invalid product type %s", pt))
	}

	propSet := make(map[string]bool)
	for _, p := range properties {
		propSet[p] = true
	}

	//if no properties have been provided - print ID, Title
	if len(propSet) == 0 {
		propSet[vangogh_local_data.IdProperty] = true
		propSet[vangogh_local_data.TitleProperty] = true
	}

	//if Title property has not been provided - add it.
	//we'll always print the title.
	//same goes for sort-by property
	propSet[vangogh_local_data.TitleProperty] = true

	//rules for collecting IDs to print:
	//1. start with user provided IDs
	//2. if createdAfter has been provided - add products created since that time
	//3. if modifiedAfter has been provided - add products modified (not by creation!) since that time
	//4. if no IDs have been collected and the request have not provided createdAfter or modifiedAfter:
	// add all product IDs

	vr, err := vangogh_local_data.NewProductReader(pt)
	if err != nil {
		return la.EndWithError(err)
	}

	if modifiedSince > 0 {
		updatedAfter, err := vr.CreatedOrUpdatedAfter(modifiedSince)
		if err != nil {
			return la.EndWithError(err)
		}

		for _, mid := range updatedAfter {
			idSet[mid] = true
		}
		if len(idSet) == 0 {
			la.EndWithResult("no new or updated %s (%s) since %v\n", pt, time.Unix(modifiedSince, 0).Format(time.Kitchen))
		}
	}

	if len(idSet) == 0 &&
		modifiedSince == 0 {
		keys, err := vr.Keys()
		if err != nil {
			return la.EndWithError(err)
		}
		for _, id := range keys {
			idSet[id] = true
		}
	}

	itp, err := vangogh_local_data.PropertyListsFromIdSet(
		maps.Keys(idSet),
		nil,
		vangogh_local_data.SupportedPropertiesOnly(pt, maps.Keys(propSet)),
		nil)

	if err != nil {
		return la.EndWithError(err)
	}

	la.EndWithSummary("", itp)

	return nil
}
