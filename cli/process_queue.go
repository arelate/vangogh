package cli

import (
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func ProcessQueueHandler(u *url.URL) error {
	return ProcessQueue(
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u))
}

func ProcessQueue(
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout) error {

	pqa := nod.Begin("processing downloads queue...")
	defer pqa.Done()

	queuedIds, err := getQueuedDownloads()
	if err != nil {
		return err
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GOGOrderDateProperty)
	if err != nil {
		return err
	}

	queuedIds, err = rdx.Sort(queuedIds, true, vangogh_integration.GOGOrderDateProperty)
	if err != nil {
		return err
	}

	if len(queuedIds) > 0 {
		if err = GetDownloads(
			queuedIds,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches,
			downloadsLayout,
			false,
			true); err != nil {
			return err
		}

		if err = Validate(
			queuedIds,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches,
			downloadsLayout,
			false); err != nil {
			return err
		}
	} else {
		pqa.EndWithResult("no downloads in the queue")
	}

	return nil
}
