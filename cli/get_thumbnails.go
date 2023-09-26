package cli

import (
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/yt_urls"
	"net/url"
	"path"
)

func GetThumbnailsHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return GetThumbnails(
		idSet,
		vangogh_local_data.FlagFromUrl(u, "missing"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetThumbnails(idSet map[string]bool, missing bool, force bool) error {
	gta := nod.NewProgress("getting thumbnails...")
	defer gta.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.VideoIdProperty,
		vangogh_local_data.MissingVideoUrlProperty,
		vangogh_local_data.MissingVideoThumbnailProperty)

	if err != nil {
		return gta.EndWithError(err)
	}

	if missing {
		missingIds, err := itemizations.MissingLocalThumbnails(rxa)
		if err != nil {
			return gta.EndWithError(err)
		}
		for id := range missingIds {
			idSet[id] = true
		}
	}

	if len(idSet) == 0 {
		if missing {
			gta.EndWithResult("all thumbnails are available locally")
		} else {
			gta.EndWithResult("no ids to get thumbnails for")
		}
		return nil
	}

	gta.TotalInt(len(idSet))

	for id := range idSet {
		videoIds, ok := rxa.GetAllValues(vangogh_local_data.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			gta.Increment()
			continue
		}

		title, _ := rxa.GetFirstVal(vangogh_local_data.TitleProperty, id)

		ta := nod.Begin("%s %s", id, title)

		dl := dolo.DefaultClient

		for _, videoId := range videoIds {

			// skipping known problematic videoIds, unless forcing download
			if !force && rxa.HasVal(vangogh_local_data.MissingVideoThumbnailProperty, id, videoId) {
				continue
			}

			gotVideoIdThumbnail := false

			for _, thumbnailUrl := range yt_urls.ThumbnailUrls(videoId) {

				_, file := path.Split(thumbnailUrl.Path)

				vta := nod.NewProgress(" %s %s", videoId, file)

				dir, err := vangogh_local_data.AbsVideoThumbnailsDirByVideoId(videoId)
				if err != nil {
					return vta.EndWithError(err)
				}

				//get-thumbnails is not using dolo.GetSetMany unlike get-images, and is downloading
				//thumbnails sequentially for two main reasons:
				//1) each thumbnail has a list of quality URLs, and we're attempting to download "the best" quality
				//moving to the next available on failure
				//2) currently dolo.GetSetMany doesn't support nod progress reporting on each individual concurrent
				//download (ok, well, StdOutPresenter doesn't, nod likely does) and for thumbnails this would mean
				//long pauses as we download individual files
				if err = dl.Download(thumbnailUrl, vta, dir, videoId+yt_urls.DefaultThumbnailExt); err != nil {
					vta.Error(err)
					continue
				}

				gotVideoIdThumbnail = true

				//yt_urls.ThumbnailUrls returns quality-sorted thumbnail urls,
				//so we can stop, if we've successfully got the best available one
				break
			}

			if !gotVideoIdThumbnail {
				if err := rxa.AddVal(vangogh_local_data.MissingVideoThumbnailProperty, id, videoId); err != nil {
					return ta.EndWithError(err)
				}
			}
		}

		ta.End()
		gta.Increment()
	}

	gta.EndWithResult("done")

	return nil
}
