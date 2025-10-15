package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
)

var UpdateSectionPermissions = map[string]author.Permission{
	vangogh_integration.UpdatesInstallers: perm.ReadOwned,
}
