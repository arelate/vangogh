package clo_delegates

import (
	"maps"
	"slices"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
)

var FuncMap = map[string]func() []string{
	"product-types":       vangogh_integration.ProductTypesCloValues,
	"image-types":         gog_integration.ImageTypesCloValues,
	"operating-systems":   vangogh_integration.OperatingSystemsCloValues,
	"downloads-layouts":   vangogh_integration.DownloadsLayoutsCloValues,
	"language-codes":      gog_integration.LanguageCodesCloValues,
	"validation-statuses": vangogh_integration.AllValidationStatusCloValues,
	"roles":               roles,
}

func roles() []string {
	rolesPermissions := perm.GetRolesPermissions()
	return slices.Collect(maps.Keys(rolesPermissions))
}
