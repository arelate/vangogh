package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
)

func InfoHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Info(
		ids,
		vangogh_integration.FlagFromUrl(u, "all-text"),
		vangogh_integration.FlagFromUrl(u, "images"),
		vangogh_integration.FlagFromUrl(u, "video-id"))
}

func Info(ids []string, allText, images, videoId bool) error {

	ia := nod.Begin("information:")
	defer ia.EndWithResult("done")

	propSet := map[string]bool{vangogh_integration.TypesProperty: true}

	for _, p := range vangogh_integration.TextProperties() {
		propSet[p] = true
	}
	if allText {
		for _, p := range vangogh_integration.AllTextProperties() {
			propSet[p] = true
		}
	}
	if images {
		imageProperties := vangogh_integration.ImageIdProperties()
		imageProperties = append(imageProperties, vangogh_integration.DehydratedImagesProperties()...)
		for _, p := range imageProperties {
			propSet[p] = true
		}
	}
	if videoId {
		for _, p := range vangogh_integration.VideoProperties() {
			propSet[p] = true
		}
	}

	rdx, err := vangogh_integration.NewReduxReader(maps.Keys(propSet)...)
	if err != nil {
		return err
	}

	itp, err := vangogh_integration.PropertyListsFromIdSet(
		ids,
		nil,
		maps.Keys(propSet),
		rdx)

	if err != nil {
		return err
	}

	ia.EndWithSummary("", itp)

	return nil
}
