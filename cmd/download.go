package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

type productImage struct {
	Image string `json:"image"`
}

func Download(
	ids []string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	dt vangogh_types.DownloadType,
	all bool) error {
	switch dt {
	case vangogh_types.ProductImage:
		return downloadProductImages(ids, pt, mt, dt, all)
	default:
		fmt.Println("download", pt, mt, dt, ids)
	}
	return nil
}

func downloadProductImages(
	ids []string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	dt vangogh_types.DownloadType,
	all bool) error {
	if !vangogh_types.SupportsDownloadType(pt, vangogh_types.ProductImage) {
		// TODO: that shouldn't be an error
		return fmt.Errorf("type %s (%s) doesn't contain %s", pt, mt, dt)
	}

	dstUrl, err := vangogh_urls.DstProductTypeUrl(pt, mt)
	if err != nil {
		return err
	}

	kvProductImages, err := kvas.NewJsonLocal(dstUrl)
	if err != nil {
		return err
	}

	if all {
		// TODO: log warning if user provided ids that would be overwritten
		ids = kvProductImages.All()
	}

	if len(ids) == 0 {
		log.Printf("no ids specified to download")
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dc := dolo.NewClient(httpClient, nil,
		&dolo.ClientOptions{
			Retries:         5,
			ResumeDownloads: true,
			Verbose:         true,
		})

	// TODO: add id to params to allow creating destination URL for product files
	dtDst, err := vangogh_urls.DstDownloadTypeUrl(dt)
	if err != nil {
		return err
	}

	for _, id := range ids {

		fmt.Printf("downloading %s for %s (%s) id %s\n", dt, pt, mt, id)

		imgRc, err := kvProductImages.Get(id)
		if err != nil {
			return err
		}

		var ii productImage
		if err := json.NewDecoder(imgRc).Decode(&ii); err != nil {
			return err
		}

		// this should be gog_urls func
		imgUrl, err := gog_urls.Image(ii.Image)
		if err != nil {
			return err
		}

		//fmt.Println(imgUrl.String())

		if err := dc.Download(imgUrl, dtDst); err != nil {
			return err
		}

		if err := imgRc.Close(); err != nil {
			return err
		}
	}

	return nil
}
