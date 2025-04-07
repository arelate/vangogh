package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/nod"
	"maps"
	"net/url"
	"os"
	"slices"
)

func UpdateDownloadsHandler(u *url.URL) error {

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	return UpdateDownloads(
		nil,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		since,
		vangogh_integration.FlagFromUrl(u, "updates-only"))
}

func UpdateDownloads(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	since int64,
	updatesOnly bool) error {

	uda := nod.Begin("itemizing updated downloads...")
	defer uda.Done()

	if ids == nil {

		updatedDetails, err := shared_data.GetDetailsUpdates(since)
		if err != nil {
			return err
		}

		ids = append(ids, slices.Collect(maps.Keys(updatedDetails))...)
	}

	if len(ids) == 0 {
		uda.EndWithResult("all downloads are up to date")
		return nil
	}

	//filter updAccountProductIds to products that have already been downloaded
	//note that this would exclude, for example, pre-order products automatic downloads
	if updatesOnly {
		rdx, err := vangogh_integration.NewReduxReader(vangogh_integration.SlugProperty)
		if err != nil {
			return err
		}

		updatesOnlyIds := make([]string, 0, len(ids))

		for _, id := range ids {

			if slug, ok := rdx.GetLastVal(vangogh_integration.SlugProperty, id); ok {
				pDir, err := vangogh_integration.AbsProductDownloadsDir(slug, downloadsLayout)
				if err != nil {
					return err
				}
				if _, err := os.Stat(pDir); os.IsNotExist(err) {
					continue
				}
			}

			updatesOnlyIds = append(updatesOnlyIds, id)
		}

		ids = updatesOnlyIds
	}

	return GetDownloads(
		ids,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		downloadsLayout,
		false,
		true)
}
