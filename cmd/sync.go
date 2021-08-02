package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/internal"
	"time"
)

func Sync(
	mt gog_media.Media,
	sinceHoursAgo int,
	data, images, screenshots, videos, downloadsUpdates, missingDownloads bool,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	verbose bool) error {

	var syncStart int64
	if sinceHoursAgo > 0 {
		syncStart = time.Now().Unix() - int64(sinceHoursAgo*60*60)
	} else {
		syncStart = time.Now().Unix()
	}

	if data {
		//get array and paged data
		paData := vangogh_products.Array()
		paData = append(paData, vangogh_products.Paged()...)
		for _, pt := range paData {
			if err := GetData(gost.NewStrSet(), nil, pt, mt, syncStart, false, false, verbose); err != nil {
				return err
			}
			fmt.Println()
		}

		//get main - detail data
		for _, pt := range vangogh_products.Detail() {
			denyIds := internal.ReadLines(vangogh_urls.Denylist(pt))
			if err := GetData(gost.NewStrSet(), denyIds, pt, mt, syncStart, true, true, verbose); err != nil {
				return err
			}
			fmt.Println()
		}

		//extract data
		if err := Extract(syncStart, mt, vangogh_properties.Extracted()); err != nil {
			return err
		}
		fmt.Println()
	}

	// get images
	if images {
		imageTypes := make([]vangogh_images.ImageType, 0, len(vangogh_images.All()))
		for _, it := range vangogh_images.All() {
			if !screenshots && it == vangogh_images.Screenshots {
				continue
			}
			imageTypes = append(imageTypes, it)
		}
		if err := GetImages(gost.NewStrSet(), imageTypes, true); err != nil {
			return err
		}
		fmt.Println()
	}

	// get videos
	if videos {
		if err := GetVideos(gost.NewStrSet(), true); err != nil {
			return err
		}
		fmt.Println()
	}

	if downloadsUpdates {
		if err := GetDownloads(
			gost.NewStrSet(),
			mt,
			operatingSystems,
			downloadTypes,
			langCodes,
			missingDownloads,
			true,
			syncStart,
			false,
			true,
			false); err != nil {
			return err
		}
	}

	// print new or updated
	return Summary(syncStart, mt)
}
