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
	it vangogh_types.ImageType,
	all bool) error {
	for _, pt := range vangogh_types.ProductTypesSupportingImageType(it) {
		if err := downloadProductType(ids, pt, mt, it, all); err != nil {
			return err
		}
	}
	return nil
}

func downloadProductType(
	ids []string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	it vangogh_types.ImageType,
	all bool) error {
	stashUrl, err := vangogh_urls.ProductTypeStashUrl(pt, mt)
	if err != nil {
		return err
	}

	propStash, err := froth.NewStash(stashUrl, vangogh_properties.FromImageType(it))
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
		log.Printf("vangogh: no ids specified to download for %s, %s (%s)", it, pt, mt)
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
	dstDir, err := vangogh_urls.DstImageTypeUrl(it)
	if err != nil {
		return err
	}

	for _, id := range ids {
		fmt.Printf("downloading %s for %s (%s) id %s\n", it, pt, mt, id)

		prop, ok := propStash.Get(id)
		if !ok || prop == "" {
			// TODO: log missing property
			log.Printf("vangogh: missing %s for %s (%s) %s\n", it, pt, mt, id)
			continue
		}

		srcUrls, err := vangogh_urls.PropImageUrl(prop, it)
		if err != nil {
			return err
		}

		for i, srcUrl := range srcUrls {
			//log.Println(srcUrl)
			if len(srcUrls) > 1 {
				fmt.Printf("- downloading %s file %d/%d\n", it, i+1, len(srcUrls))
			}
			_, err := dlClient.Download(srcUrl, dstDir)
			if err != nil {
				return err
			}
		}
	}
	return nil
}
