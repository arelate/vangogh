package cli

import (
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
)

func SizeHandler(u *url.URL) error {
	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Size(
		ids,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		vangogh_local_data.FlagFromUrl(u, "missing"),
		vangogh_local_data.FlagFromUrl(u, "all"))
}

func Size(
	ids []string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string,
	excludePatches bool,
	missing bool,
	all bool) error {

	sa := nod.NewProgress("estimating downloads size...")
	defer sa.End()

	rdx, err := vangogh_local_data.NewReduxReader(
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.DownloadStatusErrorProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	if missing {
		missingIds, err := itemizations.MissingLocalDownloads(
			rdx,
			operatingSystems,
			downloadTypes,
			langCodes,
			excludePatches)
		if err != nil {
			return sa.EndWithError(err)
		}

		if len(missingIds) == 0 {
			sa.EndWithResult("no missing downloads")
			return nil
		}

		ids = append(ids, missingIds...)
	}

	if all {
		vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
		if err != nil {
			return sa.EndWithError(err)
		}
		keys, err := vrDetails.Keys()
		if err != nil {
			return sa.EndWithError(err)
		}

		ids = append(ids, keys...)
	}

	if len(ids) == 0 {
		sa.EndWithResult("no ids to estimate size")
		return nil
	}

	sd := &sizeDelegate{}

	sa.TotalInt(len(ids))

	if err := vangogh_local_data.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		downloadTypes,
		langCodes,
		excludePatches,
		sd,
		sa); err != nil {
		return err
	}

	sa.EndWithResult("%.2fGB", sd.TotalGBsEstimate())

	return nil
}

type sizeDelegate struct {
	dlList vangogh_local_data.DownloadsList
}

func (sd *sizeDelegate) Process(_, _ string, list vangogh_local_data.DownloadsList) error {
	if sd.dlList == nil {
		sd.dlList = make(vangogh_local_data.DownloadsList, 0)
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
