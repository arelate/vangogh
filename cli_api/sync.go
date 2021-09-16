package cli_api

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cli_api/hours"
	"github.com/boggydigital/vangogh/cli_api/lines"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
	"time"
)

func SyncHandler(u *url.URL) error {
	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	data := url_helpers.Flag(u, "data")
	images := url_helpers.Flag(u, "images")
	screenshots := url_helpers.Flag(u, "screenshots")
	videos := url_helpers.Flag(u, "videos")
	downloadsUpdates := url_helpers.Flag(u, "downloads-updates")
	all := url_helpers.Flag(u, "all")
	if all {
		data, images, screenshots, videos, downloadsUpdates = true, true, true, true, true
	}

	data = data && !url_helpers.Flag(u, "no-data")
	images = images && !url_helpers.Flag(u, "no-images")
	screenshots = screenshots && !url_helpers.Flag(u, "no-screenshots")
	videos = videos && !url_helpers.Flag(u, "no-videos")
	downloadsUpdates = downloadsUpdates && !url_helpers.Flag(u, "no-downloads-updates")
	updatesOnly := url_helpers.Flag(u, "updates-only")

	sha, err := hours.Atoi(url_helpers.Value(u, "since-hours-ago"))
	if err != nil {
		return err
	}

	operatingSystems := url_helpers.OperatingSystems(u)
	downloadTypes := url_helpers.DownloadTypes(u)
	langCodes := url_helpers.Values(u, "language-code")

	return Sync(
		mt,
		sha,
		data, images, screenshots, videos, downloadsUpdates, updatesOnly,
		operatingSystems,
		downloadTypes,
		langCodes)
}

func Sync(
	mt gog_media.Media,
	sinceHoursAgo int,
	data, images, screenshots, videos, downloadsUpdates, updatesOnly bool,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string) error {

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
			if err := GetData(gost.NewStrSet(), nil, pt, mt, syncStart, false, false); err != nil {
				return err
			}
			fmt.Println()
		}

		//get main - detail data
		for _, pt := range vangogh_products.Detail() {
			denyIds := lines.Read(vangogh_urls.Denylist(pt))
			if err := GetData(gost.NewStrSet(), denyIds, pt, mt, syncStart, true, true); err != nil {
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

	// get downloads updates
	if downloadsUpdates {
		if err := UpdateDownloads(
			mt,
			operatingSystems,
			downloadTypes,
			langCodes,
			syncStart,
			updatesOnly); err != nil {
			return err
		}
		fmt.Println()
	}

	// print new or updated
	return Summary(mt, syncStart)
}
