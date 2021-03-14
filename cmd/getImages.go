package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/froth"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

func GetImages(
	ids []string,
	mt gog_types.Media,
	dt vangogh_types.DownloadType,
	all bool) error {
	for _, pt := range vangogh_types.SupportingProductTypes(dt) {
		if err := downloadProductType(ids, pt, mt, dt, all); err != nil {
			return err
		}
	}
	return nil
}

func downloadProductType(
	ids []string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	dt vangogh_types.DownloadType,
	all bool) error {
	stashUrl, err := vangogh_urls.ProductTypeStashUrl(pt, mt)
	if err != nil {
		return err
	}

	propStash, err := froth.NewStash(stashUrl, vangogh_properties.FromDownloadType(dt))
	if err != nil {
		return err
	}

	if all {
		if len(ids) > 0 {
			log.Printf("vangogh: provided would be overwritten by the 'all' flag")
		}
		ids = propStash.All()
	}

	if len(ids) == 0 {
		log.Printf("vangogh: no ids specified to download for %s, %s (%s)", dt, pt, mt)
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dlClient := dolo.NewClient(httpClient, nil,
		&dolo.ClientOptions{
			Attempts:        2,
			DelayAttempts:   5,
			ResumeDownloads: true,
			//CheckContentLength: true,
			Verbose: true,
		})

	//fmt.Println(dlClient)

	// TODO: add id to params to allow creating destination URL for product files
	dstDir, err := vangogh_urls.DstDownloadTypeUrl(dt)
	if err != nil {
		return err
	}

	for _, id := range ids {
		fmt.Printf("downloading %s for %s (%s) id %s\n", dt, pt, mt, id)

		prop, ok := propStash.Get(id)
		if !ok || prop == "" {
			// TODO: log missing property
			log.Printf("vangogh: missing %s for %s (%s) %s\n", dt, pt, mt, id)
			continue
		}

		srcUrls, err := vangogh_urls.PropDownloadUrl(prop, dt)
		if err != nil {
			return err
		}

		for i, srcUrl := range srcUrls {
			//log.Println(srcUrl)
			if len(srcUrls) > 1 {
				fmt.Printf("- downloading %s file %d/%d\n", dt, i+1, len(srcUrls))
			}
			_, err := dlClient.Download(srcUrl, dstDir)
			if err != nil {
				return err
			}
		}
	}
	return nil
}
