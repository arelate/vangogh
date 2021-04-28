package cmd

import (
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

	propExtracts, err := vangogh_properties.PropExtracts(properties)
	if err != nil {
		return err
	}

	//er, err := vangogh_extracts.NewReader(properties...)
	//if err != nil {
	//	return err
	//}

	for _, id := range ids {
		printInfo(id, false, nil, properties, propExtracts)
	}

	return nil
}
