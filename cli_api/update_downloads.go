package cli_api

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/vangogh/cli_api/hours"
	"github.com/boggydigital/vangogh/cli_api/itemize"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
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

	updatesOnly := url_helpers.Flag(u, "updates-only")

	return UpdateDownloads(mt, operatingSystems, downloadTypes, langCodes, since, updatesOnly)
}

func UpdateDownloads(
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	since int64,
	updatesOnly bool) error {

	fmt.Println("updating downloads:")

	//There are few alternatives to itemize products for downloads update:
	//1) all account-products, details - that would be too much activity for
	// very few updates (typically)
	//2) modified account-products, details - this would capture all changes,
	// including unrelated to actual product updates
	//3) modified account-products with .Updates > 0 - this should be minimal
	// required set of account-product updates that set themselves as "updated"
	// or "new" (as visible on the GOG.com account page)
	//Comparing those alternatives across runs, #3 seems pretty efficient and
	//doesn't lead to attempts to update unchanged products.
	updAccountProductIds, err := itemize.AccountProductsUpdates(mt, since)
	if err != nil {
		return err
	}

	//Additionally itemize required games for newly acquired DLCs
	requiredGamesForNewDLCs, err := itemize.RequiredAndIncluded(since)
	if err != nil {
		return err
	}

	updAccountProductIds.AddSet(requiredGamesForNewDLCs)

	if len(updAccountProductIds) == 0 {
		fmt.Printf("all downloads are up to date\n")
		return nil
	}

	//filter updAccountProductIds to products that have already been downloaded
	//note that this would exclude, for example, pre-order products automatic downloads
	if updatesOnly {
		exl, err := vangogh_extracts.NewList(vangogh_properties.SlugProperty)
		if err != nil {
			return err
		}

		for _, id := range updAccountProductIds.All() {
			ok, err := vangogh_downloads.ProductDownloaded(id, exl)
			if err != nil {
				return err
			}
			if !ok {
				updAccountProductIds.Hide(id)
			}
		}
	}

	return GetDownloads(
		updAccountProductIds,
		mt,
		operatingSystems,
		downloadTypes,
		langCodes,
		false,
		true)
}
