package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

func GetImages(
	ids []string,
	slug string,
	it vangogh_images.ImageType,
	localImageIds map[string]bool,
	missing bool) error {

	if !vangogh_images.Valid(it) {
		return fmt.Errorf("invalid image type %s", it)
	}

	imageTypeProp := vangogh_properties.FromImageType(it)
	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.SlugProperty,
		imageTypeProp)
	if err != nil {
		return err
	}

	if missing {
		if len(ids) > 0 {
			log.Printf("provided ids would be overwritten by the 'all' flag")
		}
		ids, err = allMissingLocalImageIds(exl, imageTypeProp, localImageIds)
		if err != nil {
			return err
		}
	}

	if slug != "" {
		slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
		ids = append(ids, slugIds...)
	}

	if len(ids) == 0 {
		if missing {
			fmt.Printf("all %s images are available locally\n", it)
		} else {
			fmt.Printf("no ids to get images for %s\n", it)
		}
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dl := dolo.NewClient(httpClient, nil, dolo.Defaults())

	for _, id := range ids {
		title, ok := exl.Get(vangogh_properties.TitleProperty, id)
		if !ok {
			title = id
		}
		fmt.Printf("getting %s for %s (%s)...", it, title, id)

		images, ok := exl.GetAll(imageTypeProp, id)
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
				fmt.Printf("\rgetting %s for %s (%s) file %d/%d...", it, title, id, i+1, len(srcUrls))
			}

			_, err = dl.Download(srcUrl, dstDir, "")
			if err != nil {
				return err
			}
		}

		fmt.Println("done")
	}
	return nil
}

func allMissingLocalImageIds(
	imageTypeExtracts *vangogh_extracts.ExtractsList,
	imageTypeProp string,
	localImageIds map[string]bool) ([]string, error) {

	idSet := gost.NewStrSet()
	var err error

	// filter ids to only the ones that miss that particular image type
	if localImageIds == nil {
		localImageIds, err = vangogh_urls.LocalImageIds()
		if err != nil {
			return nil, err
		}
	}

	for _, id := range imageTypeExtracts.All(imageTypeProp) {
		imageIds, ok := imageTypeExtracts.GetAll(imageTypeProp, id)
		if len(imageIds) == 0 || !ok {
			continue
		}

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

		idSet.Add(id)
	}

	return idSet.All(), err
}
