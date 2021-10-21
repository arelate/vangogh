package cli_api

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"github.com/boggydigital/vangogh/cli_api/itemize"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"github.com/boggydigital/yt_urls"
	"net/url"
)

const (
	videoExt   = ".mp4"
	missingStr = "missing"
)

func GetVideosHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	missing := url_helpers.Flag(u, "missing")

	return GetVideos(idSet, missing)
}

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

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	fmt.Println("getting videos:")

	for _, id := range idSet.All() {
		videoIds, ok := exl.GetAllRaw(vangogh_properties.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			continue
		}

		title, _ := exl.Get(vangogh_properties.TitleProperty, id)

		fmt.Printf("%s %s", id, title)

		dl := dolo.NewClient(httpClient, dolo.Defaults())

		for _, videoId := range videoIds {

			vp, err := yt_urls.GetVideoPage(videoId)
			if err != nil {
				fmt.Printf("(%s)", err)
				if addErr := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, err.Error()); addErr != nil {
					return addErr
				}
			}

			vidUrls := vp.StreamingFormats()

			if len(vidUrls) == 0 {
				if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
					return err
				}
			}

			for _, vidUrl := range vidUrls {

				if vidUrl.Url == "" {
					if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
						return err
					}
					continue
				}

				fmt.Print(".")

				dir, err := vangogh_urls.VideoDir(videoId)
				if err != nil {
					return err
				}

				u, err := url.Parse(vidUrl.Url)
				if err != nil {
					return err
				}

				_, err = dl.Download(u, dir, videoId+videoExt, nil)
				if err != nil {
					fmt.Printf("(%s)", err)
					continue
				}

				//yt_urls.StreamingUrls returns bitrate sorted video urls,
				//so we can stop, if we've successfully got the best available one
				break
			}
		}
		fmt.Print(" ")
		fmt.Println("done")
	}

	return nil
}
