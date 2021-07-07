package cmd

import (
	"fmt"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/internal"
	"github.com/boggydigital/yt_urls"
)

const (
	videoExt   = ".mp4"
	missingStr = "missing"
)

func GetVideos(ids []string, slug string, missing bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.VideoIdProperty,
		vangogh_properties.MissingVideoUrlProperty)

	if err != nil {
		return err
	}

	if missing {
		ids, err = allMissingLocalVideoIds(exl)
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

	for _, id := range ids {
		videoIds, ok := exl.GetAll(vangogh_properties.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			continue
		}

		title, _ := exl.Get(vangogh_properties.TitleProperty, id)

		fmt.Printf("getting videos for %s (%s)...", title, id)

		for _, videoId := range videoIds {

			vidUrls, err := yt_urls.StreamingUrls(videoId)
			if err != nil {
				fmt.Println(err)
				if addErr := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, err.Error()); addErr != nil {
					return addErr
				}
			}

			if len(vidUrls) == 0 {
				if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
					return err
				}
			}

			for _, vidUrl := range vidUrls {

				if vidUrl == nil || len(vidUrl.String()) == 0 {
					if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
						return err
					}
					continue
				}

				dir, err := vangogh_urls.VideoDir(videoId)
				if err != nil {
					return err
				}

				_, err = dl.Download(vidUrl, dir, videoId+videoExt)
				if err == nil {
					break
				}

				// log actual error before attempting another file
			}
		}

		fmt.Println("done")
	}

	return nil
}

func allMissingLocalVideoIds(
	exl *vangogh_extracts.ExtractsList) ([]string, error) {

	idSet := gost.NewStrSet()
	var err error

	if err := exl.AssertSupport(vangogh_properties.VideoIdProperty, vangogh_properties.MissingVideoUrlProperty); err != nil {
		return nil, err
	}

	localVideoIds, err := vangogh_urls.LocalVideoIds()
	if err != nil {
		return nil, err
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

		idSet.Add(id)
	}

	return idSet.All(), nil
}
