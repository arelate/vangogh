package clo_delegates

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli"
	"iter"
	"strings"
)

var Values = map[string]func() []string{
	"all-product-types":     allProductTypes,
	"image-types":           imageTypes,
	"redux-properties":      vangogh_integration.ReduxProperties,
	"searchable-properties": vangogh_integration.ReduxProperties,
	"operating-systems":     operatingSystems,
	"download-types":        downloadTypes,
	"downloads-layouts":     downloadsLayouts,
	"language-codes":        languageCodes,
	"sync-options":          syncOptions,
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

func downloadsLayouts() []string {
	dls := vangogh_integration.AllDownloadsLayouts()
	dlsStr := make([]string, 0, len(dls))
	for _, dl := range dls {
		dlsStr = append(dlsStr, dl.String())
	}
	return dlsStr
}

func languageCodes() []string {
	defaultLangCode := "en"
	langCodes := []string{defaultLangCode}
	for lc := range gog_integration.AllLanguageCodes() {
		if lc == defaultLangCode {
			continue
		}
		langCodes = append(langCodes, lc)
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
		cli.SyncOptionPurchases,
		cli.SyncOptionItems,
		cli.SyncOptionImages,
		cli.SyncOptionScreenshots,
		cli.SyncOptionVideosMetadata,
		cli.SyncOptionDownloadsUpdates,
	})
}
