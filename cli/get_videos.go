package cli

import (
	"errors"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/yt_urls"
	"net/http"
	"net/url"
)

func GetVideosHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return GetVideos(
		idSet,
		vangogh_local_data.FlagFromUrl(u, "missing"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetVideos(idSet map[string]bool, missing bool, force bool) error {

	gva := nod.NewProgress("getting videos...")
	defer gva.End()

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.VideoIdProperty,
		vangogh_local_data.MissingVideoUrlProperty)

	if err != nil {
		return gva.EndWithError(err)
	}

	if missing {
		missingIds, err := itemizations.MissingLocalVideos(rdx, force)
		if err != nil {
			return gva.EndWithError(err)
		}
		for id := range missingIds {
			idSet[id] = true
		}
	}

	if len(idSet) == 0 {
		if missing {
			gva.EndWithResult("all videos are available locally")
		} else {
			gva.EndWithResult("no ids to get videos for")
		}
		return nil
	}

	gva.TotalInt(len(idSet))

	for id := range idSet {
		videoIds, ok := rdx.GetAllValues(vangogh_local_data.VideoIdProperty, id)
		if !ok || len(videoIds) == 0 {
			gva.Increment()
			continue
		}

		title, _ := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)

		va := nod.Begin("%s %s", id, title)

		dl := dolo.DefaultClient

		for _, videoId := range videoIds {

			vp, err := yt_urls.GetVideoPage(http.DefaultClient, videoId)

			if err != nil {
				va.Error(err)
				if addErr := rdx.AddValues(vangogh_local_data.MissingVideoUrlProperty, videoId, err.Error()); addErr != nil {
					return addErr
				}
				continue
			}

			// handle signatureCipher separately from other errors
			if vp.SignatureCipher() {
				scErr := errors.New("signatureCipher")
				va.Error(err)
				if addErr := rdx.AddValues(vangogh_local_data.MissingVideoUrlProperty, videoId, scErr.Error()); addErr != nil {
					return addErr
				}
				continue
			}

			vfa := nod.NewProgress(" %s", vp.VideoDetails.Title)

			vidUrl := vp.BestFormat()

			if vidUrl == nil {
				if err := rdx.AddValues(vangogh_local_data.MissingVideoUrlProperty, videoId, "missing"); err != nil {
					return vfa.EndWithError(err)
				}
			}

			if vidUrl.Url == "" {
				if err := rdx.AddValues(vangogh_local_data.MissingVideoUrlProperty, videoId, "missing"); err != nil {
					return vfa.EndWithError(err)
				}
				continue
			}

			dir, err := vangogh_local_data.AbsVideoDirByVideoId(videoId)
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
			if err = dl.Download(u, force, vfa, dir, videoId+yt_urls.DefaultVideoExt); err != nil {
				vfa.Error(err)
				continue
			}

			//yt_urls.StreamingUrls returns bitrate-sorted video urls,
			//so we can stop, if we've successfully got the best available one
		}

		va.End()
		gva.Increment()
	}

	gva.EndWithResult("done")

	return nil
}
