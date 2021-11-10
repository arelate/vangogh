package v1

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
)

type productTypeMedia struct {
	productType vangogh_products.ProductType
	media       gog_media.Media
}

type productTypeMediaSort struct {
	productTypeMedia
	sort string
	desc bool
}

var exl *vangogh_extracts.ExtractsList
var valueReaders map[productTypeMedia]*vangogh_values.ValueReader
var sortedIds map[productTypeMediaSort][]string
var defaultSort = vangogh_properties.TitleProperty

func Init() error {
	var err error

	exl, err = vangogh_extracts.NewList(vangogh_properties.Extracted()...)
	if err != nil {
		return err
	}

	valueReaders = make(map[productTypeMedia]*vangogh_values.ValueReader)
	mt := gog_media.Game
	for _, pt := range vangogh_products.Local() {
		ptm := productTypeMedia{productType: pt, media: mt}
		valueReaders[ptm], err = vangogh_values.NewReader(pt, mt)
		if err != nil {
			return err
		}
	}

	//TODO: consider priming that with default sort for a type
	sortedIds = make(map[productTypeMediaSort][]string)

	return nil
}
