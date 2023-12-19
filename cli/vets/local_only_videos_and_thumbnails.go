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

	rdx, err := vangogh_local_data.ReduxReader(vangogh_local_data.VideoIdProperty)
	if err != nil {
		return lova.EndWithError(err)
	}

	ieva := nod.NewProgress(" itemizing expected videos...")
	ids := rdx.Keys(vangogh_local_data.VideoIdProperty)

	ieva.TotalInt(len(ids))

	expectedVideos := make(map[string]bool)
	for _, id := range ids {
		videoIds, ok := rdx.GetAllValues(vangogh_local_data.VideoIdProperty, id)
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

		avp, err := vangogh_local_data.GetAbsDir(vangogh_local_data.Videos)
		if err != nil {
			return flova.EndWithError(err)
		}

		avtp, err := vangogh_local_data.GetAbsRelDir(vangogh_local_data.VideoThumbnails)
		if err != nil {
			return flova.EndWithError(err)
		}

		for _, videoId := range unexpectedVideos {
			absLocalVideoPath, err := vangogh_local_data.AbsLocalVideoPath(videoId)
			if err != nil {
				return flova.EndWithError(err)
			}
			nod.Log("removing local only videoId=%s file=%s", videoId, absLocalVideoPath)
			if err := vangogh_local_data.MoveToRecycleBin(avp, absLocalVideoPath); err != nil && !os.IsNotExist(err) {
				return flova.EndWithError(err)
			}
			absLocalThumbnailPath, err := vangogh_local_data.AbsLocalVideoThumbnailPath(videoId)
			if err != nil {
				return flova.EndWithError(err)
			}
			nod.Log("removing local only thumbnail videoId=%s file=%s", videoId, absLocalThumbnailPath)
			if err := vangogh_local_data.MoveToRecycleBin(avtp, absLocalThumbnailPath); err != nil && !os.IsNotExist(err) {
				return flova.EndWithError(err)
			}
			flova.Increment()
		}
		flova.EndWithResult("done")
	}

	return nil
}
