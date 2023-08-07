package clo_delegates

import (
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh_local_data"
	"strings"
)

var Values = map[string]func() []string{
	"remote-product-types":  remoteProductTypes,
	"local-product-types":   localProductTypes,
	"image-types":           imageTypes,
	"redux-properties":      vangogh_local_data.ReduxProperties,
	"all-properties":        vangogh_local_data.AllProperties,
	"searchable-properties": vangogh_local_data.ReduxProperties,
	"digestible-properties": vangogh_local_data.DigestibleProperties,
	"operating-systems":     operatingSystems,
	"download-types":        downloadTypes,
	"language-codes":        languageCodes,
	"sync-options":          syncOptions,
	"vet-options":           vetOptions,
}

func productTypeStr(productTypes []vangogh_local_data.ProductType) []string {
	ptsStr := make([]string, 0, len(productTypes))
	for _, pt := range productTypes {
		ptsStr = append(ptsStr, pt.String())
	}
	return ptsStr
}

func remoteProductTypes() []string {
	return productTypeStr(vangogh_local_data.RemoteProducts())
}

func localProductTypes() []string {
	return productTypeStr(vangogh_local_data.LocalProducts())
}

func imageTypes() []string {
	its := vangogh_local_data.AllImageTypes()
	itsStr := make([]string, 0, len(its))
	for _, it := range its {
		itsStr = append(itsStr, it.String())
	}
	return itsStr
}

func operatingSystems() []string {
	oss := vangogh_local_data.AllOperatingSystems()
	ossStr := make([]string, 0, len(oss))
	for _, os := range oss {
		ossStr = append(ossStr, strings.ToLower(os.String()))
	}
	return ossStr
}

func downloadTypes() []string {
	dts := vangogh_local_data.AllDownloadTypes()
	dtsStr := make([]string, 0, len(dts))
	for _, dt := range dts {
		dtsStr = append(dtsStr, dt.String())
	}
	return dtsStr
}

func languageCodes() []string {
	defaultLangCode := "en"
	langCodes := []string{defaultLangCode}
	rxa, err := vangogh_local_data.ConnectReduxAssets(vangogh_local_data.LanguageNameProperty)
	if err != nil {
		return langCodes
	}
	for _, lc := range rxa.Keys(vangogh_local_data.LanguageNameProperty) {
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
		cli.SyncOptionItems,
		cli.SyncOptionImages,
		cli.SyncOptionScreenshots,
		cli.SyncOptionVideos,
		cli.SyncOptionThumbnails,
		cli.SyncOptionDownloadsUpdates,
	})
}

func vetOptions() []string {
	return options([]string{
		cli.VetOptionLocalOnlyData,
		cli.VetOptionLocalOnlyImages,
		cli.VetOptionLocalOnlyVideosAndThumbnails,
		cli.VetOptionRecycleBin,
		cli.VetOptionInvalidData,
		cli.VetOptionUnresolvedManualUrls,
		cli.VetOptionInvalidResolvedManualUrls,
		cli.VetOptionMissingChecksums,
		cli.VetStaleDehydrations,
		cli.VetOldLogs,
		cli.VetOldBackups,
	})
}
