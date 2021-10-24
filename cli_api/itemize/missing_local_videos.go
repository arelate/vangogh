package itemize

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
)

type videoPropertiesGetter struct {
	extractsList *vangogh_extracts.ExtractsList
}

func NewVideoPropertiesGetter(exl *vangogh_extracts.ExtractsList) *videoPropertiesGetter {
	return &videoPropertiesGetter{
		extractsList: exl,
	}
}

func (vpg *videoPropertiesGetter) GetVideoIds(id string) ([]string, bool) {
	return vpg.extractsList.GetAllRaw(vangogh_properties.VideoIdProperty, id)
}

func (vpg *videoPropertiesGetter) IsMissingVideo(videoId string) bool {
	return vpg.extractsList.Contains(vangogh_properties.MissingVideoUrlProperty, videoId)
}

func MissingLocalVideos(exl *vangogh_extracts.ExtractsList) (gost.StrSet, error) {
	all := exl.All(vangogh_properties.VideoIdProperty)

	localVideoSet, err := vangogh_urls.LocalVideoIds()
	if err != nil {
		return nil, err
	}

	vpg := NewVideoPropertiesGetter(exl)

	mlva := nod.NewProgress(" itemizing local videos...")
	defer mlva.EndWithResult("done")

	return missingLocalFiles(all, localVideoSet, vpg.GetVideoIds, vpg.IsMissingVideo, mlva)
}
