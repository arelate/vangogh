package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/vangogh/internal"
)

func Sync(mt gog_types.Media, verbose bool) error {
	// get paginated data
	for _, pt := range vangogh_types.AllPagedProductTypes() {
		if err := GetData(nil, nil, pt, mt, false, verbose); err != nil {
			return err
		}
	}

	// get main - detail data
	for _, pt := range vangogh_types.AllDetailProductTypes() {
		denyIds := internal.ReadLines(vangogh_urls.DenylistUrl(pt))
		if err := GetData(nil, denyIds, pt, mt, true, verbose); err != nil {
			return err
		}
	}

	// extract data
	for _, pt := range vangogh_types.AllLocalProductTypes() {
		if err := Stash(pt, mt, vangogh_properties.AllStashedProperties()); err != nil {
			return err
		}
		if err := Distill(pt, mt, vangogh_properties.AllStashedProperties()); err != nil {
			return err
		}
	}

	// get images
	for _, dt := range vangogh_types.AllImageDownloadTypes() {
		if err := GetImages(nil, mt, dt, true); err != nil {
			return err
		}
	}

	return nil
}
