package cli

import (
	"net/url"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func SizeHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	q := u.Query()

	return Size(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		q.Has(vangogh_integration.UrlNoDlcsParameter),
		q.Has(vangogh_integration.UrlNoExtrasParameter),
		q.Has(vangogh_integration.UrlNoPatchesParameter),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		q.Has(vangogh_integration.UrlMissingParameter),
		q.Has(vangogh_integration.UrlDebugParameter),
		q.Has(vangogh_integration.UrlAllParameter))
}

func Size(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noDlcs bool,
	noExtras bool,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	missing bool,
	debug bool,
	all bool) error {

	sa := nod.NewProgress("estimating downloads size...")
	defer sa.Done()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, noDlcs, noExtras, noPatches)

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogSlugProperty,
		vangogh_integration.GogProductTypeProperty,
		vangogh_integration.GogManualUrlFilenameProperty,
		vangogh_integration.DownloadStatusErrorProperty)
	if err != nil {
		return err
	}

	if missing {
		missingIds, err := itemizations.MissingLocalDownloads(
			rdx,
			operatingSystems,
			langCodes,
			noDlcs,
			noExtras,
			noPatches,
			downloadsLayout,
			debug)
		if err != nil {
			return err
		}

		if len(missingIds) == 0 {
			sa.EndWithResult("no missing downloads")
			return nil
		}

		ids = append(ids, missingIds...)
	}

	if all {

		var gogDetailsDir string
		gogDetailsDir, err = vangogh_integration.AbsProductTypeDir(vangogh_integration.GogDetails)
		if err != nil {
			return err
		}

		var kvGogDetails kevlar.KeyValues
		kvGogDetails, err = kevlar.New(gogDetailsDir, kevlar.JsonExt)
		if err != nil {
			return err
		}

		for id := range kvGogDetails.Keys() {
			ids = append(ids, id)
		}
	}

	if len(ids) == 0 {
		sa.EndWithResult("no ids to estimate size")
		return nil
	}

	sd := &sizeDelegate{}

	sa.TotalInt(len(ids))

	if err = vangogh_integration.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		langCodes,
		noDlcs,
		noExtras,
		noPatches,
		sd,
		sa); err != nil {
		return err
	}

	sa.EndWithResult("%.2fGB", sd.TotalGBsEstimate())

	return nil
}

type sizeDelegate struct {
	dlList vangogh_integration.DownloadsList
}

func (sd *sizeDelegate) Process(_, _ string, list vangogh_integration.DownloadsList) error {
	if sd.dlList == nil {
		sd.dlList = make(vangogh_integration.DownloadsList, 0)
	}
	sd.dlList = append(sd.dlList, list...)
	return nil
}

func (sd *sizeDelegate) TotalGBsEstimate() float64 {
	if sd.dlList != nil {
		return sd.dlList.TotalGBsEstimate()
	}
	return 0
}
