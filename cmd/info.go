package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
)

func Info(idSet gost.StrSet, allText, images, videoId bool) error {

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
		return err
	}

	return Print(
		idSet.All(),
		nil,
		propSet.All(),
		exl)
}
