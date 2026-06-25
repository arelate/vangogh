package compton_data

import (
	"iter"
	"slices"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
)

var PropertyPermissions = map[string]author.Permission{
	vangogh_integration.GogTagIdProperty:         perm.ReadTagId,
	vangogh_integration.VangoghLocalTagsProperty: perm.ReadLocalTags,
	vangogh_integration.GogOwnedProperty:         perm.ReadOwned,
	vangogh_integration.GogUserWishlistProperty:  perm.ReadWishlist,
}

var PropertyActionPermissions = map[string]author.Permission{
	vangogh_integration.GogTagIdProperty:         perm.WriteTagId,
	vangogh_integration.VangoghLocalTagsProperty: perm.WriteLocalTags,
	vangogh_integration.GogUserWishlistProperty:  perm.WriteWishlist,
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
