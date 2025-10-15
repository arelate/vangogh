package compton_data

import (
	"iter"
	"slices"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
)

var PropertyPermissions = map[string]author.Permission{
	vangogh_integration.TagIdProperty:     perm.ReadTagId,
	vangogh_integration.LocalTagsProperty: perm.ReadLocalTags,
}

var PropertyActionPermissions = map[string]author.Permission{
	vangogh_integration.TagIdProperty:     perm.WriteTagId,
	vangogh_integration.LocalTagsProperty: perm.WriteLocalTags,
}

func PermittedProperties(properties []string, permissions ...author.Permission) iter.Seq[string] {
	return func(yield func(string) bool) {

		for _, p := range properties {

			if prm, ok := PropertyPermissions[p]; ok && !slices.Contains(permissions, prm) {
				continue
			}

			if !yield(p) {
				return
			}
		}
	}
}
