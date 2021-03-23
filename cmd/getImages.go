package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/froth"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

func GetImages(
	ids []string,
	it vangogh_images.ImageType,
	all bool) error {

	if !vangogh_images.Valid(it) {
		return fmt.Errorf("invalid image type %s", it)
	}

	propExtracts, err := froth.NewStash(
		vangogh_urls.ExtractsDir(),
		vangogh_properties.FromImageType(it))
	if err != nil {
		return err
	}

	if all {
		if len(ids) > 0 {
			log.Printf("provided ids would be overwritten by the 'all' flag")
		}
		ids = propExtracts.All()
	}

	if len(ids) == 0 {
		log.Printf("missing ids to get images for %s", it)
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dlClient := dolo.NewClient(httpClient, nil,
		&dolo.ClientOptions{
			Attempts:        3,
			DelayAttempts:   5,
			ResumeDownloads: true,
			//CheckContentLength: true,
			Verbose: true,
		})

	//fmt.Println(dlClient)

	for _, id := range ids {
		log.Printf("get %s id %s", it, id)

		prop, ok := propExtracts.Get(id)
		if !ok || prop == "" {
			log.Printf("missing %s id %s", it, id)
			continue
		}

		srcUrls, err := vangogh_urls.PropImageUrls(prop.(string), it)
		if err != nil {
			return err
		}

		for i, srcUrl := range srcUrls {

			dstDir, err := vangogh_urls.ImageDir(srcUrl.Path)

			if len(srcUrls) > 1 {
				log.Printf("get %s id %s file %d/%d", it, id, i+1, len(srcUrls))
			}

			_, err = dlClient.Download(srcUrl, dstDir)
			if err != nil {
				return err
			}

			//fmt.Println(srcUrl, dstDir)
		}
	}
	return nil
}
