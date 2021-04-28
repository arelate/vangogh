package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

func Info(ids []string, mt gog_media.Media, images, videoId bool) error {
	var err error
	productTypeReaders := make(map[vangogh_products.ProductType]*vangogh_values.ValueReader)
	for _, pt := range vangogh_products.Local() {
		productTypeReaders[pt], err = vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}
	}

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

	for _, id := range ids {
		printInfo(id, false, nil, properties, propExtracts)
	}

	return nil
}
