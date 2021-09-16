package url_helpers

import (
	"github.com/arelate/vangogh_downloads"
	"net/url"
)

func DownloadTypes(u *url.URL) []vangogh_downloads.DownloadType {
	dtStrings := Values(u, "download-type")
	return vangogh_downloads.ParseManyDownloadTypes(dtStrings)
}
