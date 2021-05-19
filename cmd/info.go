package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
)

func Info(ids map[string]bool, images, videoId bool) error {

	properties := map[string]bool{
		vangogh_properties.TypesProperty: true,
	}
	for _, tp := range vangogh_properties.Text() {
		properties[tp] = true
	}
	if images {
		for _, ip := range vangogh_properties.ImageId() {
			properties[ip] = true
		}
	}
	if videoId {
		for _, vp := range vangogh_properties.VideoId() {
			properties[vp] = true
		}
	}

	exl, err := vangogh_extracts.NewList(properties)
	if err != nil {
		return err
	}

	for id, ok := range ids {
		if !ok {
			continue
		}
		printInfo(id, nil, properties, exl)
	}

	return nil
}
