package cmd

import (
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/vangogh/internal"
)

func Sync(mt gog_types.Media) error {
	//sync paginated product types
	for _, pt := range vangogh_types.AllPagedProductTypes() {
		if err := Fetch(nil, nil, pt, mt, false); err != nil {
			return err
		}
	}

	//sync main - detail missing product types
	for _, pt := range vangogh_types.AllDetailProductTypes() {
		denyIds := internal.ReadLines(vangogh_urls.DenylistUrl(pt))
		if err := Fetch(nil, denyIds, pt, mt, true); err != nil {
			return err
		}
	}

	// stash and distill properties
	for _, pt := range vangogh_types.AllLocalProductTypes() {
		if err := Stash(pt, mt, vangogh_properties.AllStashedProperties()); err != nil {
			return err
		}
		if err := Distill(pt, mt, vangogh_properties.AllStashedProperties()); err != nil {
			return err
		}
	}

	// download images
	for _, pt := range vangogh_types.AllLocalProductTypes() {
		for _, dt := range vangogh_types.AllImageDownloadTypes() {
			if err := Download(nil, pt, mt, dt, true); err != nil {
				return err
			}
		}
	}

	return nil
}
