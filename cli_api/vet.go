package cli_api

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/checks"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
)

func VetHandler(u *url.URL) error {
	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	operatingSystems := url_helpers.OperatingSystems(u)
	downloadTypes := url_helpers.DownloadTypes(u)
	langCodes := url_helpers.Values(u, "language-code")

	fix := url_helpers.Flag(u, "fix")
	return Vet(mt, operatingSystems, downloadTypes, langCodes, fix)
}

func Vet(
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	fix bool) error {

	sda := nod.Begin("vetting local data...")
	defer sda.End()

	if err := checks.LocalOnlySplitProducts(mt, fix); err != nil {
		return sda.EndWithError(err)
	}

	if err := checks.FilesInRecycleBin(fix); err != nil {
		return sda.EndWithError(err)
	}

	if err := checks.InvalidLocalProductData(mt, fix); err != nil {
		return sda.EndWithError(err)
	}

	if err := checks.UnresolvedManualUrls(mt, operatingSystems, downloadTypes, langCodes, fix); err != nil {
		return sda.EndWithError(err)
	}

	//products with values different from extracts
	//images that are not linked to a product
	//videos that are not linked to a product
	//logs older than 30 days
	//checksum file errors

	return nil
}
