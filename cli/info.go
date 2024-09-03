package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
)

func InfoHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return Info(
		idSet,
		vangogh_local_data.FlagFromUrl(u, "all-text"),
		vangogh_local_data.FlagFromUrl(u, "images"),
		vangogh_local_data.FlagFromUrl(u, "video-id"))
}

func Info(idSet map[string]bool, allText, images, videoId bool) error {

	ia := nod.Begin("information:")
	defer ia.End()

	propSet := map[string]bool{vangogh_local_data.TypesProperty: true}

	for _, p := range vangogh_local_data.TextProperties() {
		propSet[p] = true
	}
	if allText {
		for _, p := range vangogh_local_data.AllTextProperties() {
			propSet[p] = true
		}
	}
	if images {
		imageProperties := vangogh_local_data.ImageIdProperties()
		imageProperties = append(imageProperties, vangogh_local_data.DehydratedImagesProperties()...)
		for _, p := range imageProperties {
			propSet[p] = true
		}
	}
	if videoId {
		for _, p := range vangogh_local_data.VideoIdProperties() {
			propSet[p] = true
		}
	}

	rdx, err := vangogh_local_data.NewReduxReader(maps.Keys(propSet)...)
	if err != nil {
		return ia.EndWithError(err)
	}

	itp, err := vangogh_local_data.PropertyListsFromIdSet(
		maps.Keys(idSet),
		nil,
		maps.Keys(propSet),
		rdx)

	if err != nil {
		return ia.EndWithError(err)
	}

	ia.EndWithSummary("", itp)

	return nil
}
