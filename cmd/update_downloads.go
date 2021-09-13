package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/hours"
	"github.com/boggydigital/vangogh/cmd/itemize"
	"github.com/boggydigital/vangogh/cmd/url_helpers"
	"net/url"
	"time"
)

func UpdateDownloadsHandler(u *url.URL) error {
	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	operatingSystems := url_helpers.OperatingSystems(u)
	downloadTypes := url_helpers.DownloadTypes(u)
	langCodes := url_helpers.Values(u, "language-code")

	sha, err := hours.Atoi(url_helpers.Value(u, "since-hours-ago"))
	if err != nil {
		return err
	}
	since := time.Now().Unix() - int64(sha*60*60)

	return UpdateDownloads(mt, operatingSystems, downloadTypes, langCodes, since)
}

func UpdateDownloads(
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	since int64) error {

	fmt.Println("finding updated details and account-products")

	idSet := gost.NewStrSet()

	idSet, err := itemize.All(idSet, true, true, since, vangogh_products.Details, mt)
	if err != nil {
		return err
	}

	return GetDownloads(idSet, mt, operatingSystems, downloadTypes, langCodes, false, true)
}
