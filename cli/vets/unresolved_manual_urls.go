package vets

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func UnresolvedManualUrls(
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	fix bool) error {

	cumu := nod.NewProgress("checking unresolved manual-urls...")
	defer cumu.End()

	vangogh_local_data.PrintParams(nil, operatingSystems, langCodes, downloadTypes, noPatches)

	rdx, err := vangogh_local_data.NewReduxReader(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.LocalManualUrlProperty)
	if err != nil {
		return cumu.EndWithError(err)
	}

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return cumu.EndWithError(err)
	}

	allDetails, err := vrDetails.Keys()
	if err != nil {
		return cumu.EndWithError(err)
	}
	unresolvedIds := make(map[string]bool)

	cumu.TotalInt(len(allDetails))
	for _, id := range allDetails {

		det, err := vrDetails.Details(id)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList, err := vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList = downloadsList.Only(operatingSystems, langCodes, downloadTypes, noPatches)

		for _, dl := range downloadsList {
			if _, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl); !ok {
				unresolvedIds[id] = true
			}
		}

		cumu.Increment()
	}

	if len(unresolvedIds) == 0 {
		cumu.EndWithResult("all good")
	} else {

		summary, err := vangogh_local_data.PropertyListsFromIdSet(
			maps.Keys(unresolvedIds),
			nil,
			[]string{vangogh_local_data.TitleProperty},
			rdx)

		heading := fmt.Sprintf("found %d problems:", len(unresolvedIds))
		if fix {
			heading = "found problems (run get-downloads to fix):"
		}

		if err != nil {
			return cumu.EndWithError(err)
		}
		cumu.EndWithSummary(heading, summary)
	}

	return nil
}
