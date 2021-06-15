package internal

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
)

var CloValuesDelegates = map[string]func() []string{
	"media":                 media,
	"remote-product-types":  remoteProductTypes,
	"local-product-types":   localProductTypes,
	"image-types":           imageTypes,
	"extracted-properties":  vangogh_properties.Extracted,
	"all-properties":        vangogh_properties.All,
	"searchable-properties": vangogh_properties.Searchable,
	"digestible-properties": vangogh_properties.Digestible,
	"sortable-properties":   vangogh_properties.Sortable,
}

func productTypeStr(productTypes []vangogh_products.ProductType) []string {
	ptsStr := make([]string, 0, len(productTypes))
	for _, pt := range productTypes {
		ptsStr = append(ptsStr, pt.String())
	}
	return ptsStr
}

func remoteProductTypes() []string {
	return productTypeStr(vangogh_products.Remote())
}

func localProductTypes() []string {
	return productTypeStr(vangogh_products.Local())
}

func media() []string {
	media := gog_media.All()
	mediaStr := make([]string, 0, len(media))
	for _, md := range media {
		mediaStr = append(mediaStr, md.String())
	}
	return mediaStr
}

func imageTypes() []string {
	its := vangogh_images.All()
	itsStr := make([]string, 0, len(its))
	for _, it := range its {
		itsStr = append(itsStr, it.String())
	}
	return itsStr
}
