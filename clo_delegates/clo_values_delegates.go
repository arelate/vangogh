package clo_delegates

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli"
	"iter"
	"slices"
	"strings"
)

var Values = map[string]func() []string{
	"all-product-types":     allProductTypes,
	"image-types":           imageTypes,
	"redux-properties":      vangogh_integration.ReduxProperties,
	"searchable-properties": vangogh_integration.ReduxProperties,
	"operating-systems":     operatingSystems,
	"download-types":        downloadTypes,
	"language-codes":        languageCodes,
	"sync-options":          syncOptions,
	"vet-options":           vetOptions,
}

func allProductTypes() []string {
	return productTypeStr(vangogh_integration.AllProductTypes())
}

func productTypeStr(productTypes iter.Seq[vangogh_integration.ProductType]) []string {
	ptsStr := make([]string, 0)
	for pt := range productTypes {
		ptsStr = append(ptsStr, pt.String())
	}
	return ptsStr
}

func imageTypes() []string {
	its := vangogh_integration.AllImageTypes()
	itsStr := make([]string, 0, len(its))
	for _, it := range its {
		itsStr = append(itsStr, it.String())
	}
	return itsStr
}

func operatingSystems() []string {
	oss := vangogh_integration.AllOperatingSystems()
	ossStr := make([]string, 0, len(oss))
	for _, os := range oss {
		ossStr = append(ossStr, strings.ToLower(os.String()))
	}
	return ossStr
}

func downloadTypes() []string {
	dts := vangogh_integration.AllDownloadTypes()
	dtsStr := make([]string, 0, len(dts))
	for _, dt := range dts {
		dtsStr = append(dtsStr, dt.String())
	}
	return dtsStr
}

func languageCodes() []string {
	//defaultLangCode := "en"
	//langCodes := []string{defaultLangCode}
	//rdx, err := vangogh_integration.NewReduxReader(vangogh_integration.LanguageNameProperty)
	//if err != nil {
	//	return langCodes
	//}
	//for lc := range rdx.Keys(vangogh_integration.LanguageNameProperty) {
	//	if lc == defaultLangCode {
	//		continue
	//	}
	//	langCodes = append(langCodes, strings.ToLower(lc))
	//}
	//return langCodes
	return slices.Collect(gog_integration.AllLanguageCodes())
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
		cli.SyncOptionItems,
		cli.SyncOptionImages,
		cli.SyncOptionScreenshots,
		cli.SyncOptionVideosMetadata,
		cli.SyncOptionDownloadsUpdates,
	})
}

func vetOptions() []string {
	return options([]string{
		cli.VetLocalOnlyImages,
		cli.VetRecycleBin,
		cli.VetInvalidData,
		cli.VetUnresolvedManualUrls,
		cli.VetInvalidResolvedManualUrls,
		cli.VetMissingChecksums,
		cli.VetStaleDehydrations,
		cli.VetOldLogs,
	})
}
