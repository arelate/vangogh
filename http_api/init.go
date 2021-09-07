package http_api

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

var exl *vangogh_extracts.ExtractsList
var gameValueReaders map[vangogh_products.ProductType]*vangogh_values.ValueReader

func Init() error {
	var err error

	exl, err = vangogh_extracts.NewList(vangogh_properties.Extracted()...)
	if err != nil {
		return err
	}

	gameValueReaders = make(map[vangogh_products.ProductType]*vangogh_values.ValueReader)
	for _, pt := range vangogh_products.Local() {
		gameValueReaders[pt], err = vangogh_values.NewReader(pt, gog_media.Game)
		if err != nil {
			return err
		}
	}

	return nil
}
