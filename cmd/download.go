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

		srcUrls, err := propertyToUrls(dt, prop)
		if err != nil {
			return err
		}

		for _, srcUrl := range srcUrls {
			//fmt.Println(srcUrl.String(), dstDir)
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
		fallthrough
	case vangogh_types.BoxArt:
		fallthrough
	case vangogh_types.BackgroundImage:
		fallthrough
	case vangogh_types.GalaxyBackgroundImage:
		fallthrough
	case vangogh_types.Logo:
		fallthrough
	case vangogh_types.Icon:
		boxArtUrl, err := gog_urls.Image(prop)
		if err != nil {
			return urls, err
		}
		urls = append(urls, boxArtUrl)
	}

	return urls, nil
}
