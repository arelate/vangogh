package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/internal"
	"log"
)

func GetImages(
	idSet gost.StrSet,
	its []vangogh_images.ImageType,
	missing bool) error {

	for _, it := range its {
		if !vangogh_images.Valid(it) {
			return fmt.Errorf("invalid image type %s", it)
		}
	}

	propSet := gost.NewStrSetWith(vangogh_properties.TitleProperty, vangogh_properties.SlugProperty)

	for _, it := range its {
		propSet.Add(vangogh_properties.FromImageType(it))
	}

	exl, err := vangogh_extracts.NewList(propSet.All()...)
	if err != nil {
		return err
	}

	//for every product we'll collect image types missing for id and download only those
	idMissingTypes := map[string][]vangogh_images.ImageType{}

	if missing {
		if idSet.Len() > 0 {
			log.Printf("provided ids would be overwritten by 'missing' flag")
		}

		localImageSet, err := vangogh_urls.LocalImageIds()
		if err != nil {
			return err
		}
		//to track image types missing for each product we do the following:
		//1. for every image type requested to be downloaded - get product ids that don't have this image type locally
		//2. for every product id we get this way - add this image type to idMissingTypes[id]
		for _, it := range its {
			//1
			missingImageIds, err := itemize.MissingLocalImages(it, exl, localImageSet)
			if err != nil {
				return err
			}

			//2
			for id := range missingImageIds {
				if idMissingTypes[id] == nil {
					idMissingTypes[id] = make([]vangogh_images.ImageType, 0)
				}
				idMissingTypes[id] = append(idMissingTypes[id], it)
			}
		}
	} else {
		for id := range idSet {
			idMissingTypes[id] = its
		}
	}

	if len(idMissingTypes) == 0 {
		if missing {
			fmt.Printf("all images are available locally\n")
		} else {
			fmt.Printf("need at least one product id to get images\n")
		}
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dl := dolo.NewClient(httpClient, nil, dolo.Defaults())

	for id, missingIts := range idMissingTypes {
		title, ok := exl.Get(vangogh_properties.TitleProperty, id)
		if !ok {
			title = id
		}

		fmt.Printf("getting images for %s (%s) - ", title, id)

		for _, it := range missingIts {

			fmt.Printf("%s...", it)

			images, ok := exl.GetAll(vangogh_properties.FromImageType(it), id)
			if !ok || len(images) == 0 {
				fmt.Printf("(missing)...")
				continue
			}

			srcUrls, err := vangogh_urls.PropImageUrls(images, it)
			if err != nil {
				return err
			}

			for i, srcUrl := range srcUrls {

				if len(srcUrls) > 0 && i > 0 {
					fmt.Print(".")
				}
				dstDir, err := vangogh_urls.ImageDir(srcUrl.Path)

				_, err = dl.Download(srcUrl, dstDir, "")
				if err != nil {
					fmt.Println(err)
					continue
				}
			}
		}

		fmt.Println("done")
	}
	return nil
}
