package url_helpers

import (
	"github.com/arelate/vangogh_downloads"
	"net/url"
)

func OperatingSystems(u *url.URL) []vangogh_downloads.OperatingSystem {
	osStrings := Values(u, "operating-system")
	return vangogh_downloads.ParseManyOperatingSystems(osStrings)
}
