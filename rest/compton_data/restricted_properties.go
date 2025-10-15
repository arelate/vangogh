package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
)

var RestrictedProperties = map[string]author.Permission{
	vangogh_integration.TagIdProperty:     perm.ReadTagId,
	vangogh_integration.LocalTagsProperty: perm.ReadLocalTags,
}

var RestrictedPropertiesActions = map[string]author.Permission{
	vangogh_integration.TagIdProperty:     perm.WriteTagId,
	vangogh_integration.LocalTagsProperty: perm.WriteLocalTags,
}
