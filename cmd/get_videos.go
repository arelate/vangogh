package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/vangogh/internal"
	"github.com/boggydigital/yt_urls"
)

const videoExt = ".mp4"

func GetVideos(ids map[string]bool, all bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.VideoIdProperty,
		vangogh_properties.MissingVideoUrlProperty)

	if err != nil {
		return err
	}

	if all {
		ids, err = allMissingLocalVideoIds(exl)
		if err != nil {
			return err
		}
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	dl := dolo.NewClient(httpClient, nil,
		&dolo.ClientOptions{
			Attempts:        3,
			DelayAttempts:   5,
			ResumeDownloads: true,
			MinSizeComplete: 512,
			//CheckContentLength: true,
			//Verbose: true,
		})

	for id, _ := range ids {
		videoIds, ok := exl.GetAll(vangogh_properties.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			continue
		}

		title, _ := exl.Get(vangogh_properties.TitleProperty, id)

		fmt.Printf("get videos for %s (%s)\n", title, id)

		for _, videoId := range videoIds {

			vidUrl, err := yt_urls.BestStreamingUrl(videoId)
			if err != nil {
				return err
			}

			if vidUrl == nil || len(vidUrl.String()) == 0 {
				if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, ""); err != nil {
					return err
				}

				continue
			}

			dir, err := vangogh_urls.VideoDir(videoId)
			if err != nil {
				return err
			}

			_, err = dl.Download(vidUrl, dir, videoId+videoExt)
			if err != nil {
				return err
			}
		}
	}

	return nil
}

func allMissingLocalVideoIds(
	exl *vangogh_extracts.ExtractsList) (ids map[string]bool, err error) {

	ids = make(map[string]bool, 0)

	if err := exl.AssertSupport(vangogh_properties.VideoIdProperty, vangogh_properties.MissingVideoUrlProperty); err != nil {
		return ids, err
	}

	localVideoIds, err := vangogh_urls.LocalVideoIds()
	if err != nil {
		return ids, err
	}

	for _, id := range exl.All(vangogh_properties.VideoIdProperty) {

		videoIds, ok := exl.GetAll(vangogh_properties.VideoIdProperty, id)
		if len(videoIds) == 0 || !ok {
			continue
		}

		haveVideos := true
		for _, videoId := range videoIds {
			if localVideoIds[videoId] {
				continue
			}
			if exl.Contains(vangogh_properties.MissingVideoUrlProperty, videoId) {
				continue
			}
			haveVideos = false
		}

		if haveVideos {
			continue
		}

		ids[id] = true
	}

	return ids, nil
}
