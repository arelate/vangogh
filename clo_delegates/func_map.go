package clo_delegates

import (
	"maps"
	"slices"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli"
	"github.com/arelate/vangogh/perm"
)

var FuncMap = map[string]func() []string{
	"all-product-types": vangogh_integration.ProductTypesCloValues,
	"image-types":       vangogh_integration.ImageTypesCloValues,
	"redux-properties":  vangogh_integration.ReduxProperties,
	"operating-systems": vangogh_integration.OperatingSystemsCloValues,
	"download-types":    vangogh_integration.DownloadTypesCloValues,
	"downloads-layouts": vangogh_integration.DownloadsLayoutsCloValues,
	"language-codes":    gog_integration.LanguageCodesCloValues,
	"sync-options":      syncOptions,
	"roles":             roles,
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
		cli.SyncOptionDescriptionImages,
		cli.SyncOptionImages,
		cli.SyncOptionScreenshots,
		cli.SyncOptionVideosMetadata,
		cli.SyncOptionDownloadsUpdates,
		cli.SynOptionBinaries,
	})
}

func roles() []string {
	rolesPermissions := perm.GetRolesPermissions()
	return slices.Collect(maps.Keys(rolesPermissions))
}
