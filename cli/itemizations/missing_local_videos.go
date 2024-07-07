package itemizations

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

type videoPropertiesGetter struct {
	rdx kevlar.ReadableRedux
}

func NewVideoPropertiesGetter(rdx kevlar.ReadableRedux) *videoPropertiesGetter {
	return &videoPropertiesGetter{
		rdx: rdx,
	}
}

func (vpg *videoPropertiesGetter) GetVideoIds(id string) ([]string, bool) {
	return vpg.rdx.GetAllValues(vangogh_local_data.VideoIdProperty, id)
}

func (vpg *videoPropertiesGetter) IsMissingVideo(videoId string) bool {
	return vpg.rdx.HasKey(vangogh_local_data.MissingVideoUrlProperty, videoId)
}

func missingLocalVideoRelatedFiles(
	rdx kevlar.ReadableRedux,
	localVideoIdsDelegate func() (map[string]bool, error),
	excludeKnownMissingVideos bool,
	videoFilesDesc string) (map[string]bool, error) {

	all := rdx.Keys(vangogh_local_data.VideoIdProperty)

	localSet, err := localVideoIdsDelegate()
	if err != nil {
		return map[string]bool{}, err
	}

	vpg := NewVideoPropertiesGetter(rdx)

	mlma := nod.NewProgress(" itemizing local %s...", videoFilesDesc)
	defer mlma.EndWithResult("done")

	//some video related files would benefit from skipping known missing (local) videos,
	//for example actual local video files - we don't won't attempting again and again.
	//some video related files actually don't need to skip
	//for example video thumbnails, where we want to show a YouTube link with a thumbnail
	//even if the file is not present locally
	var excludeDelegate func(videoId string) bool
	if excludeKnownMissingVideos {
		excludeDelegate = vpg.IsMissingVideo
	}

	return missingLocalFiles(all, localSet, vpg.GetVideoIds, excludeDelegate, mlma)
}

func MissingLocalVideos(rdx kevlar.ReadableRedux, force bool) (map[string]bool, error) {
	return missingLocalVideoRelatedFiles(
		rdx,
		vangogh_local_data.LocalVideoIds,
		!force,
		"videos")
}

func MissingLocalThumbnails(rdx kevlar.ReadableRedux) (map[string]bool, error) {
	return missingLocalVideoRelatedFiles(
		rdx,
		vangogh_local_data.LocalVideoThumbnailIds,
		false,
		"thumbnails")
}
