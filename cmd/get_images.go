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
	"strings"
)

func GetImages(
	ids []string,
	it vangogh_images.ImageType,
	localImageIds map[string]bool,
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
		ids, err = findIdsMissingImages(it, propExtracts, localImageIds)
		if err != nil {
			return err
		}
	}

	if len(ids) == 0 {
		if all {
			log.Printf("all %s images are available locally", it)
		} else {
			log.Printf("missing ids to get images for %s", it)
		}
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
			MinSizeComplete: 512,
			//CheckContentLength: true,
			//Verbose: true,
		})

	//fmt.Println(dlClient)

	for _, id := range ids {
		log.Printf("get %s id %s", it, id)

		prop, ok := propExtracts.Get(id)
		if !ok || prop == "" {
			log.Printf("missing %s id %s", it, id)
			continue
		}

		srcUrls, err := vangogh_urls.PropImageUrls(prop, it)
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

func findIdsMissingImages(it vangogh_images.ImageType, propExtracts *froth.Stash, localImageIds map[string]bool) (ids []string, err error) {

	ids = make([]string, 0)

	// filter ids to only the ones that miss that particular image type
	if localImageIds == nil {
		localImageIds, err = vangogh_urls.LocalImageIds()
		if err != nil {
			return ids, err
		}
	}

	for _, id := range propExtracts.All() {
		prop, ok := propExtracts.Get(id)
		if prop == "" || !ok {
			continue
		}
		if localImageIds[prop] {
			continue
		}
		if it == vangogh_images.Screenshots {
			haveAllScr := true
			for _, scr := range strings.Split(prop, ",") {
				if !localImageIds[scr] {
					haveAllScr = false
					break
				}
			}
			if haveAllScr {
				continue
			}
		}
		ids = append(ids, id)
	}

	return ids, err
}
