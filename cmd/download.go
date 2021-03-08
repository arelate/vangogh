package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/froth"
	"github.com/boggydigital/vangogh/internal"
	"log"
	"net/url"
)

func Download(
	ids []string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	dt vangogh_types.DownloadType,
	all bool) error {

	if !vangogh_types.SupportsDownloadType(pt, dt) {
		log.Printf("type %s (%s) doesn't contain %s", pt, mt, dt)
		return nil
	}

	stashUrl, err := vangogh_urls.ProductTypeStashUrl(pt, mt)
	if err != nil {
		return err
	}

	propStash, err := froth.NewStash(stashUrl, vangogh_properties.FromDownloadType(dt))
	if err != nil {
		return err
	}

	if all {
		// TODO: log warning if user provided ids that would be overwritten
		ids = propStash.All()
	}

	if len(ids) == 0 {
		log.Printf("no ids specified to download")

		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dlClient := dolo.NewClient(httpClient, nil,
		&dolo.ClientOptions{
			Retries:         5,
			ResumeDownloads: true,
			Verbose:         true,
		})

	// TODO: add id to params to allow creating destination URL for product files
	dstDir, err := vangogh_urls.DstDownloadTypeUrl(dt)
	if err != nil {
		return err
	}

	for _, id := range ids {
		fmt.Printf("downloading %s for %s (%s) id %s\n", dt, pt, mt, id)

		prop, ok := propStash.Get(id)
		if !ok {
			// TODO: log missing property
			continue
		}

		srcUrls, err := propertyToUrls(dt, prop)
		if err != nil {
			return err
		}

		for _, srcUrl := range srcUrls {
			//fmt.Println(srcUrl.String())
			if err := dlClient.Download(srcUrl, dstDir); err != nil {
				return err
			}
		}
	}

	return nil
}

func propertyToUrls(dt vangogh_types.DownloadType, prop string) ([]*url.URL, error) {
	urls := make([]*url.URL, 0)

	switch dt {
	case vangogh_types.Image:
		imgUrl, err := gog_urls.Image(prop)
		if err != nil {
			return urls, err
		}
		urls = append(urls, imgUrl)
	}

	return urls, nil
}
