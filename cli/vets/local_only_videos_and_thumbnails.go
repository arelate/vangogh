package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"os"
)

func LocalOnlyVideosAndThumbnails(fix bool) error {

	lova := nod.Begin("checking for local only videos...")
	defer lova.End()

	ilva := nod.Begin(" itemizing local videos...")
	localVideos, err := vangogh_local_data.LocalVideoIds()
	if err != nil {
		ilva.End()
		return lova.EndWithError(err)
	}
	ilva.EndWithResult("done")

	rxa, err := vangogh_local_data.ConnectReduxAssets(vangogh_local_data.VideoIdProperty)
	if err != nil {
		return lova.EndWithError(err)
	}

	ieva := nod.NewProgress(" itemizing expected videos...")
	ids := rxa.Keys(vangogh_local_data.VideoIdProperty)

	ieva.TotalInt(len(ids))

	expectedVideos := make(map[string]bool)
	for _, id := range ids {
		videoIds, ok := rxa.GetAllValues(vangogh_local_data.VideoIdProperty, id)
		if !ok {
			ieva.Increment()
			continue
		}
		for _, videoId := range videoIds {
			if videoId == "" {
				continue
			}
			expectedVideos[videoId] = true
		}
		ieva.Increment()
	}

	ieva.EndWithResult("done")

	unexpectedVideos := make([]string, 0, len(expectedVideos))
	for videoId := range localVideos {
		if videoId == "" {
			continue
		}
		if !expectedVideos[videoId] {
			unexpectedVideos = append(unexpectedVideos, videoId)
		}
	}

	lova.EndWithResult("found %d unexpected videos", len(unexpectedVideos))

	if fix && len(unexpectedVideos) > 0 {
		flova := nod.NewProgress(" removing %d local only video(s)...", len(unexpectedVideos))
		flova.TotalInt(len(unexpectedVideos))

		for _, videoId := range unexpectedVideos {
			absLocalVideoPath := vangogh_local_data.AbsLocalVideoPath(videoId)
			nod.Log("removing local only videoId=%s file=%s", videoId, absLocalVideoPath)
			if err := vangogh_local_data.MoveToRecycleBin(vangogh_local_data.AbsVideosDir(), absLocalVideoPath); err != nil && !os.IsNotExist(err) {
				return flova.EndWithError(err)
			}
			absLocalThumbnailPath := vangogh_local_data.AbsLocalVideoThumbnailPath(videoId)
			nod.Log("removing local only thumbnail videoId=%s file=%s", videoId, absLocalThumbnailPath)
			if err := vangogh_local_data.MoveToRecycleBin(vangogh_local_data.AbsVideoThumbnailsDir(), absLocalThumbnailPath); err != nil && !os.IsNotExist(err) {
				return flova.EndWithError(err)
			}
			flova.Increment()
		}
		flova.EndWithResult("done")
	}

	return nil
}
