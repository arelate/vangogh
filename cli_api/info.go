package cli_api

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/expand"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
)

func InfoHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	allText := url_helpers.Flag(u, "all-text")
	images := url_helpers.Flag(u, "images")
	videoId := url_helpers.Flag(u, "video-id")

	return Info(idSet, allText, images, videoId)
}

func Info(idSet gost.StrSet, allText, images, videoId bool) error {

	ia := nod.Begin("information:")
	defer ia.End()

	propSet := gost.NewStrSetWith(vangogh_properties.TypesProperty)

	propSet.Add(vangogh_properties.Text()...)
	if allText {
		propSet.Add(vangogh_properties.AllText()...)
	}
	if images {
		propSet.Add(vangogh_properties.ImageId()...)
	}
	if videoId {
		propSet.Add(vangogh_properties.VideoId()...)
	}

	exl, err := vangogh_extracts.NewList(propSet.All()...)
	if err != nil {
		return ia.EndWithError(err)
	}

	itp, err := expand.IdsToPropertyLists(
		"",
		idSet.All(),
		nil,
		propSet.All(),
		exl)

	if err != nil {
		return ia.EndWithError(err)
	}

	ia.EndWithSummary(itp)

	return nil
}
