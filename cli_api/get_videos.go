package cli_api

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
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

	gva := nod.NewProgress("getting videos...")
	defer gva.End()

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.VideoIdProperty,
		vangogh_properties.MissingVideoUrlProperty)

	if err != nil {
		return gva.EndWithError(err)
	}

	if missing {
		missingIds, err := itemize.MissingLocalVideos(exl)
		if err != nil {
			return gva.EndWithError(err)
		}
		idSet.AddSet(missingIds)
	}

	if idSet.Len() == 0 {
		if missing {
			gva.EndWithResult("all videos are available locally")
		} else {
			gva.EndWithResult("no ids to get videos for")
		}
		return nil
	}

	httpClient, err := http_client.Default()
	if err != nil {
		return gva.EndWithError(err)
	}

	gva.TotalInt(idSet.Len())

	for _, id := range idSet.All() {
		videoIds, ok := exl.GetAllRaw(vangogh_properties.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			gva.Increment()
			continue
		}

		title, _ := exl.Get(vangogh_properties.TitleProperty, id)

		va := nod.Begin("%s %s", id, title)

		dl := dolo.NewClient(httpClient, dolo.Defaults())

		for _, videoId := range videoIds {

			vp, err := yt_urls.GetVideoPage(videoId)
			if err != nil {
				va.Error(err)
				if addErr := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, err.Error()); addErr != nil {
					return addErr
				}
				continue
			}

			vfa := nod.NewProgress(" %s", vp.Title())

			vidUrls := vp.StreamingFormats()

			if len(vidUrls) == 0 {
				if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
					return vfa.EndWithError(err)
				}
			}

			for _, vidUrl := range vidUrls {

				if vidUrl.Url == "" {
					if err := exl.Add(vangogh_properties.MissingVideoUrlProperty, videoId, missingStr); err != nil {
						return vfa.EndWithError(err)
					}
					continue
				}

				dir, err := vangogh_urls.VideoDir(videoId)
				if err != nil {
					return vfa.EndWithError(err)
				}

				u, err := url.Parse(vidUrl.Url)
				if err != nil {
					return vfa.EndWithError(err)
				}

				//get-videos is not using dolo.GetSetMany unlike get-images, and is downloading
				//videos sequentially for two main reasons:
				//1) each video has a list of bitrate-sorted URLs, and we're attempting to download "the best" quality
				//moving to the next available on failure
				//2) currently dolo.GetSetMany doesn't support nod progress reporting on each individual concurrent
				//download (ok, well, StdOutPresenter doesn't, nod likely does) and for video files this would mean
				//long pauses as we download individual files
				_, err = dl.Download(u, dir, videoId+videoExt, vfa)
				if err != nil {
					vfa.Error(err)
					continue
				}

				//yt_urls.StreamingUrls returns bitrate-sorted video urls,
				//so we can stop, if we've successfully got the best available one
				break
			}
		}

		va.End()
		gva.Increment()
	}

	//gva.EndWithResult("done")

	return nil
}
