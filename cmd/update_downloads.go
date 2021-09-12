package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/hours"
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

	idSet := gost.NewStrSet()

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return err
	}

	vrAccountProducts, err := vangogh_values.NewReader(vangogh_products.AccountProducts, mt)
	if err != nil {
		return err
	}

	idSet.Add(vrDetails.ModifiedAfter(since, false)...)
	idSet.Add(vrAccountProducts.ModifiedAfter(since, false)...)

	return GetDownloads(idSet, mt, operatingSystems, downloadTypes, langCodes, false, true)
}
