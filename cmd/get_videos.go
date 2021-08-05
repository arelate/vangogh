package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/internal"
	"github.com/boggydigital/yt_urls"
)

const (
	videoExt   = ".mp4"
	missingStr = "missing"
)

func GetVideos(idSet gost.StrSet, missing bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.VideoIdProperty,
		vangogh_properties.MissingVideoUrlProperty)

	if err != nil {
		return err
	}

	if missing {
		missingIds, err := itemize.MissingLocalVideos(exl)
		if err != nil {
			return err
		}
		idSet.AddSet(missingIds)
	}

	if idSet.Len() == 0 {
		if missing {
			fmt.Println("all videos are available locally")
		} else {
			fmt.Println("no ids to get videos for")
		}
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dl := dolo.NewClient(httpClient, nil, dolo.Defaults())

	for _, id := range idSet.All() {
		videoIds, ok := exl.GetAllRaw(vangogh_properties.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			continue
		}

		title, _ := exl.Get(vangogh_properties.TitleProperty, id)

		fmt.Printf("getting videos for %s (%s)...", title, id)

		for _, videoId := range videoIds {

			vidUrls, err := yt_urls.StreamingUrls(videoId)
			if err != nil {
				fmt.Printf("(%s)...", err)
				if addErr := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, err.Error()); addErr != nil {
					return addErr
				}
			}

			if len(vidUrls) == 0 {
				if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
					return err
				}
			}

			for i, vidUrl := range vidUrls {

				if vidUrl == nil || len(vidUrl.String()) == 0 {
					if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
						return err
					}
					continue
				}

				if len(vidUrls) > 0 && i > 0 {
					fmt.Print(".")
				}

				dir, err := vangogh_urls.VideoDir(videoId)
				if err != nil {
					return err
				}

				_, err = dl.Download(vidUrl, dir, videoId+videoExt)
				if err != nil {
					fmt.Printf("(%s)...", err)
					continue
				}
			}
		}

		fmt.Println("done")
	}

	return nil
}
