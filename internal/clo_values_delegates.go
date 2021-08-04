package internal

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"strings"
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
	"operating-systems":     operatingSystems,
	"download-types":        downloadTypes,
	"language-codes":        languageCodes,
	"sync-items":            syncItems,
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

func operatingSystems() []string {
	oss := vangogh_downloads.AllOperatingSystems()
	ossStr := make([]string, 0, len(oss))
	for _, os := range oss {
		ossStr = append(ossStr, strings.ToLower(os.String()))
	}
	return ossStr
}

func downloadTypes() []string {
	dts := vangogh_downloads.AllDownloadTypes()
	dtsStr := make([]string, 0, len(dts))
	for _, dt := range dts {
		dtsStr = append(dtsStr, dt.String())
	}
	return dtsStr
}

func languageCodes() []string {
	defaultLangCode := "en"
	langCodes := []string{defaultLangCode}
	exl, err := vangogh_extracts.NewList(vangogh_properties.LanguageNameProperty)
	if err != nil {
		return langCodes
	}
	for _, lc := range exl.All(vangogh_properties.LanguageNameProperty) {
		if lc == defaultLangCode {
			continue
		}
		langCodes = append(langCodes, strings.ToLower(lc))
	}
	return langCodes
}

func syncItems() []string {
	items := []string{
		"data",
		"images",
		"screenshots",
		"videos",
		"downloads-updates",
	}

	excludeItems := make([]string, len(items))
	for i, item := range items {
		excludeItems[i] = "no-" + item
	}

	items = append(items, "all")
	return append(items, excludeItems...)
}
