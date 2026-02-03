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
		vangogh_integration.DownloadTypesFromUrl(u),
		q.Has("no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		q.Has("missing"),
		q.Has("debug"),
		q.Has("all"))
}

func Size(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	missing bool,
	debug bool,
	all bool) error {

	sa := nod.NewProgress("estimating downloads size...")
	defer sa.Done()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.SlugProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.ManualUrlFilenameProperty,
		vangogh_integration.DownloadStatusErrorProperty)
	if err != nil {
		return err
	}

	if missing {
		missingIds, err := itemizations.MissingLocalDownloads(
			rdx,
			operatingSystems,
			downloadTypes,
			langCodes,
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

		detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
		if err != nil {
			return err
		}

		kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
		if err != nil {
			return err
		}

		for id := range kvDetails.Keys() {
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
		downloadTypes,
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
