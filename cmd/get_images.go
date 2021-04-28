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
	localImageIds map[string]bool,
	all bool) error {

	if !vangogh_images.Valid(it) {
		return fmt.Errorf("invalid image type %s", it)
	}

	titleExtracts, err := froth.NewStash(
		vangogh_urls.ExtractsDir(),
		vangogh_properties.TitleProperty)

	imageTypeExtracts, err := froth.NewStash(
		vangogh_urls.ExtractsDir(),
		vangogh_properties.FromImageType(it))

	if err != nil {
		return err
	}

	if all {
		if len(ids) > 0 {
			log.Printf("provided ids would be overwritten by the 'all' flag")
		}
		ids, err = findIdsMissingImages(imageTypeExtracts, localImageIds)
		if err != nil {
			return err
		}
	}

	if len(ids) == 0 {
		if all {
			fmt.Printf("all %s images are available locally\n", it)
		} else {
			fmt.Printf("missing ids to get images for %s\n", it)
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
		title, ok := titleExtracts.Get(id)
		if !ok {
			title = id
		}
		fmt.Printf("get %s for %s (%s)\n", it, title, id)

		images, ok := imageTypeExtracts.GetAll(id)
		if !ok || len(images) == 0 {
			fmt.Printf("missing %s for %s (%s)\n", it, title, id)
			continue
		}

		srcUrls, err := vangogh_urls.PropImageUrls(images, it)
		if err != nil {
			return err
		}

		for i, srcUrl := range srcUrls {

			dstDir, err := vangogh_urls.ImageDir(srcUrl.Path)

			if len(srcUrls) > 1 {
				fmt.Printf("get %s for %s (%s) file %d/%d\n", it, title, id, i+1, len(srcUrls))
			}

			_, err = dlClient.Download(srcUrl, dstDir)
			if err != nil {
				return err
			}
		}
	}
	return nil
}

func findIdsMissingImages(imageTypeExtracts *froth.Stash, localImageIds map[string]bool) (ids []string, err error) {

	ids = make([]string, 0)

	// filter ids to only the ones that miss that particular image type
	if localImageIds == nil {
		localImageIds, err = vangogh_urls.LocalImageIds()
		if err != nil {
			return ids, err
		}
	}

	for _, id := range imageTypeExtracts.All() {
		imageIds, ok := imageTypeExtracts.GetAll(id)
		if len(imageIds) == 0 || !ok {
			continue
		}
		//if len(imageIds) > 1 {
		//	fmt.Println(id, imageIds)
		//}
		haveImages := true
		for _, imageId := range imageIds {
			if localImageIds[imageId] {
				continue
			}
			haveImages = false
		}
		if haveImages {
			continue
		}
		ids = append(ids, id)
	}

	return ids, err
}
