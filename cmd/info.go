package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
)

func Info(slug string, ids []string, allText, images, videoId bool) error {

	idSet := gost.StrSetWith(ids...)

	propSet := gost.StrSetWith(vangogh_properties.TypesProperty)
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

	for _, id := range idSet.All() {
		if err := printInfo(id, nil, propSet.All(), exl); err != nil {
			return err
		}
	}

	return nil
}
