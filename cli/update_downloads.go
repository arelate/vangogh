package cli

import (
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
)

func UpdateDownloadsHandler(u *url.URL) error {

	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return err
	}

	return UpdateDownloads(
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		since,
		vangogh_local_data.FlagFromUrl(u, "updates-only"))
}

func UpdateDownloads(
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	since int64,
	updatesOnly bool) error {

	uda := nod.Begin("itemizing updated downloads...")
	defer uda.End()

	//Here is a set of items we'll consider as updated for updating downloads:
	//1) account-products Updates, all products that have .IsNew or .Updates > 0 -
	// basically items that GOG.com marked as new/updated
	//2) required games for newly acquired license-products -
	// making sure we update downloads for base product, when purchasing a DLC separately
	//3) modified details (since certain time) -
	// this accounts for interrupted sync, when we already processed account-products
	// Updates (so .IsNew or .Updates > 0 won't be true anymore) and have updated
	// details as a result. This is somewhat excessive for general case, however would
	// allow us to capture all updated account-products at a price of some extra checks
	updAccountProductIds := make(map[string]bool)

	uapIds, err := itemizations.AccountProductsUpdates()
	if err != nil {
		return uda.EndWithError(err)
	}

	for _, id := range uapIds {
		updAccountProductIds[id] = true
	}

	//Additionally itemize required games for newly acquired DLCs
	requiredGamesForNewDLCs, err := itemizations.RequiredAndIncluded(since)
	if err != nil {
		return uda.EndWithError(err)
	}

	for _, rg := range requiredGamesForNewDLCs {
		updAccountProductIds[rg] = true
	}

	//Additionally add modified details in case the sync was interrupted and
	//account-products doesn't have .IsNew or .Updates > 0 items
	modifiedDetails, err := itemizations.Modified(since, vangogh_local_data.Details)
	if err != nil {
		return uda.EndWithError(err)
	}

	for _, md := range modifiedDetails {
		updAccountProductIds[md] = true
	}

	if len(updAccountProductIds) == 0 {
		uda.EndWithResult("all downloads are up to date")
		return nil
	}

	//filter updAccountProductIds to products that have already been downloaded
	//note that this would exclude, for example, pre-order products automatic downloads
	if updatesOnly {
		rdx, err := vangogh_local_data.NewReduxReader(vangogh_local_data.SlugProperty)
		if err != nil {
			return uda.EndWithError(err)
		}

		for id := range updAccountProductIds {
			ok, err := vangogh_local_data.IsProductDownloaded(id, rdx)
			if err != nil {
				return uda.EndWithError(err)
			}
			if !ok {
				delete(updAccountProductIds, id)
			}
		}
	}

	uda.EndWithResult("done")

	return GetDownloads(
		maps.Keys(updAccountProductIds),
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		false,
		true)
}
