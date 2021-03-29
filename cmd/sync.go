package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/vangogh/internal"
	"log"
	"time"
)

func Sync(mt gog_media.Media, noData, images, screenshots, verbose bool) error {

	syncStart := time.Now().Unix()

	if !noData {
		// get paginated data
		for _, pt := range vangogh_products.AllPaged() {
			if err := GetData(nil, nil, pt, mt, syncStart, false, verbose); err != nil {
				return err
			}
		}

		// get main - detail data
		for _, pt := range vangogh_products.AllDetail() {
			denyIds := internal.ReadLines(vangogh_urls.Denylist(pt))
			if err := GetData(nil, denyIds, pt, mt, syncStart, true, verbose); err != nil {
				return err
			}
		}

		// extract data
		if err := Extract(mt, vangogh_properties.AllExtracted()); err != nil {
			return err
		}
	}

	localImageIds, err := vangogh_urls.LocalImageIds()
	if err != nil {
		return err
	}
	// get images
	for _, it := range vangogh_images.All() {
		if !images ||
			(!screenshots && it == vangogh_images.Screenshots) {
			continue
		}
		if err := GetImages(nil, it, localImageIds, true); err != nil {
			return err
		}
	}

	// TODO: get files

	// print created, modified
	return reportCreatedModifiedAfter(syncStart, mt)

}

func reportCreatedModifiedAfter(timestamp int64, mt gog_media.Media) error {
	for _, pt := range vangogh_products.AllLocal() {
		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}

		createdIds := vr.CreatedAfter(timestamp)
		if len(createdIds) > 0 {
			log.Printf("%s (%s) created this sync:", pt, mt)
			if err := List(nil, timestamp, pt, mt); err != nil {
				return err
			}
		} else {
			log.Printf("no %s (%s) created this sync", pt, mt)
		}

		modifiedIds := vr.CreatedAfter(timestamp)
		modifiedNotCreatedIds := make([]string, 0)
		for _, modId := range modifiedIds {
			if !stringsContain(createdIds, modId) {
				modifiedNotCreatedIds = append(modifiedNotCreatedIds, modId)
			}
		}

		if len(modifiedNotCreatedIds) > 0 {
			log.Printf("%s (%s) modified this sync:", pt, mt)
			if err := List(nil, timestamp, pt, mt); err != nil {
				return err
			}
		} else {
			log.Printf("no %s (%s) modified this sync", pt, mt)
		}
	}

	log.Println("sync took:", time.Since(time.Unix(timestamp, 0)).String())
	return nil
}
