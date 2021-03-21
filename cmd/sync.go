package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/vangogh/internal"
	"log"
	"time"
)

func Sync(mt gog_types.Media, noData, images, screenshots, verbose bool) error {

	syncStart := time.Now().Unix()

	if !noData {
		// get paginated data
		for _, pt := range vangogh_types.AllPagedProductTypes() {
			if err := GetData(nil, nil, pt, mt, syncStart, false, verbose); err != nil {
				return err
			}
		}

		// get main - detail data
		for _, pt := range vangogh_types.AllDetailProductTypes() {
			denyIds := internal.ReadLines(vangogh_urls.Denylist(pt))
			if err := GetData(nil, denyIds, pt, mt, syncStart, true, verbose); err != nil {
				return err
			}
		}

		// extract data
		if err := Extract(mt, vangogh_properties.AllExtractedProperties()); err != nil {
			return err
		}
	}

	// get images
	for _, it := range vangogh_types.AllImageTypes() {
		if !images ||
			(!screenshots && it == vangogh_types.Screenshots) {
			continue
		}
		if err := GetImages(nil, it, true); err != nil {
			return err
		}
	}

	// TODO: get files

	// print created, modified
	return reportCreatedModifiedAfter(syncStart, mt)

}

func reportCreatedModifiedAfter(timestamp int64, mt gog_types.Media) error {
	for _, pt := range vangogh_types.AllLocalProductTypes() {
		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		createdIds := vr.CreatedAfter(timestamp)
		if len(createdIds) > 0 {
			log.Printf("%s (%s) created this sync:", pt, mt)
			if err := List(createdIds, pt, mt); err != nil {
				return err
			}
		} else {
			log.Printf("no %s (%s) created this sync", pt, mt)
		}

		modifiedIds := vr.CreatedAfter(timestamp)
		modifiedNotCreatedIds := make([]string, 0)
		for _, modId := range modifiedIds {
			modifiedByCreation := false
			for _, crId := range createdIds {
				if modId == crId {
					modifiedByCreation = true
					break
				}
			}
			if !modifiedByCreation {
				modifiedNotCreatedIds = append(modifiedNotCreatedIds, modId)
			}
		}

		if len(modifiedNotCreatedIds) > 0 {
			log.Printf("%s (%s) modified this sync:", pt, mt)
			if err := List(modifiedNotCreatedIds, pt, mt); err != nil {
				return err
			}
		} else {
			log.Printf("no %s (%s) modified this sync", pt, mt)
		}
	}
	return nil
}
