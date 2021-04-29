package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
)

func Info(ids []string, images, videoId bool) error {

	properties := []string{vangogh_properties.TypesProperty}
	properties = append(properties, vangogh_properties.Text()...)
	if images {
		properties = append(properties, vangogh_properties.ImageId()...)
	}
	if videoId {
		properties = append(properties, vangogh_properties.VideoIdProperty)
	}

	exl, err := vangogh_extracts.NewList(properties...)
	if err != nil {
		return err
	}

	for _, id := range ids {
		printInfo(id, false, nil, properties, exl)
	}

	return nil
}
