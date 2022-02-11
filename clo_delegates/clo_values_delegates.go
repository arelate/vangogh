package clo_delegates

import (
	"github.com/arelate/gog_atu"
	"github.com/arelate/vangogh_api/cli"
	"github.com/arelate/vangogh_data"
	"strings"
)

var Values = map[string]func() []string{
	"media":                 media,
	"remote-product-types":  remoteProductTypes,
	"local-product-types":   localProductTypes,
	"image-types":           imageTypes,
	"extracted-properties":  vangogh_data.Extracted,
	"all-properties":        vangogh_data.AllProperties,
	"searchable-properties": vangogh_data.Searchable,
	"digestible-properties": vangogh_data.Digestible,
	"operating-systems":     operatingSystems,
	"download-types":        downloadTypes,
	"language-codes":        languageCodes,
	"sync-options":          syncOptions,
	"vet-options":           vetOptions,
	"temp-directory":        getTempDir,
}

var tempDir = ""

func productTypeStr(productTypes []vangogh_data.ProductType) []string {
	ptsStr := make([]string, 0, len(productTypes))
	for _, pt := range productTypes {
		ptsStr = append(ptsStr, pt.String())
	}
	return ptsStr
}

func remoteProductTypes() []string {
	return productTypeStr(vangogh_data.RemoteProducts())
}

func localProductTypes() []string {
	return productTypeStr(vangogh_data.LocalProducts())
}

func media() []string {
	media := gog_atu.AllMedia()
	mediaStr := make([]string, 0, len(media))
	for _, md := range media {
		mediaStr = append(mediaStr, md.String())
	}
	return mediaStr
}

func imageTypes() []string {
	its := vangogh_data.AllImageTypes()
	itsStr := make([]string, 0, len(its))
	for _, it := range its {
		itsStr = append(itsStr, it.String())
	}
	return itsStr
}

func operatingSystems() []string {
	oss := vangogh_data.AllOperatingSystems()
	ossStr := make([]string, 0, len(oss))
	for _, os := range oss {
		ossStr = append(ossStr, strings.ToLower(os.String()))
	}
	return ossStr
}

func downloadTypes() []string {
	dts := vangogh_data.AllDownloadTypes()
	dtsStr := make([]string, 0, len(dts))
	for _, dt := range dts {
		dtsStr = append(dtsStr, dt.String())
	}
	return dtsStr
}

func languageCodes() []string {
	defaultLangCode := "en"
	langCodes := []string{defaultLangCode}
	rxa, err := vangogh_data.ConnectReduxAssets(vangogh_data.LanguageNameProperty)
	if err != nil {
		return langCodes
	}
	for _, lc := range rxa.Keys(vangogh_data.LanguageNameProperty) {
		if lc == defaultLangCode {
			continue
		}
		langCodes = append(langCodes, strings.ToLower(lc))
	}
	return langCodes
}

func options(opts []string) []string {
	excludeOptions := make([]string, len(opts))
	for i, opt := range opts {
		excludeOptions[i] = cli.NegOpt(opt)
	}

	opts = append([]string{"all"}, opts...)
	return append(opts, excludeOptions...)
}

func syncOptions() []string {
	return options([]string{
		cli.SyncOptionData,
		cli.SyncOptionImages,
		cli.SyncOptionScreenshots,
		cli.SyncOptionVideos,
		cli.SyncOptionDownloadsUpdates,
	})
}

func vetOptions() []string {
	return options([]string{
		cli.VetOptionLocalOnlyData,
		cli.VetOptionRecycleBin,
		cli.VetOptionInvalidData,
		cli.VetOptionUnresolvedManualUrls,
	})
}

func SetTempDir(td string) {
	tempDir = td
}

func getTempDir() []string {
	return []string{
		tempDir,
	}
}
