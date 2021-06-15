package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
)

func Info(slug string, ids []string, allText, images, videoId bool, sortBy string, desc bool) error {

	idSet := gost.StrSetWith(ids...)

	propSet := gost.StrSetWith(vangogh_properties.TypesProperty, sortBy)
	if slug != "" {
		propSet.Add(vangogh_properties.SlugProperty)
	}
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

	if slug != "" {
		slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
		idSet.Add(slugIds...)
	}

	return Print(
		idSet.All(),
		nil,
		propSet.All(),
		sortBy,
		desc,
		exl)
}
