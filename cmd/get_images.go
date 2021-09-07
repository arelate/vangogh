package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/http_client"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/cmd/url_helpers"
	"net/url"
)

func GetImagesHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	imageTypes := url_helpers.Values(u, "image-type")
	its := make([]vangogh_images.ImageType, 0, len(imageTypes))
	for _, imageType := range imageTypes {
		its = append(its, vangogh_images.Parse(imageType))
	}
	missing := url_helpers.Flag(u, "missing")

	return GetImages(idSet, its, missing)
}

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

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	dl := dolo.NewClient(httpClient, nil, dolo.Defaults())

	fmt.Println("get images:")

	for id, missingIts := range idMissingTypes {
		title, ok := exl.Get(vangogh_properties.TitleProperty, id)
		if !ok {
			title = id
		}

		fmt.Printf("%s %s - ", id, title)

		for _, it := range missingIts {

			images, ok := exl.GetAll(vangogh_properties.FromImageType(it), id)
			if !ok || len(images) == 0 {
				fmt.Printf("(missing)...")
				continue
			}

			srcUrls, err := vangogh_urls.PropImageUrls(images, it)
			if err != nil {
				return err
			}

			fmt.Printf("%s", it)

			for _, srcUrl := range srcUrls {

				fmt.Print(".")
				dstDir, err := vangogh_urls.ImageDir(srcUrl.Path)

				_, err = dl.Download(srcUrl, dstDir, "")
				if err != nil {
					fmt.Println(err)
					continue
				}
			}

			fmt.Print(" ")
		}

		fmt.Println("done")
	}
	return nil
}
