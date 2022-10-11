package vets

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func UnresolvedManualUrls(
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string,
	fix bool) error {

	cumu := nod.NewProgress("checking unresolved manual-urls...")
	defer cumu.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.LocalManualUrlProperty)
	if err != nil {
		return cumu.EndWithError(err)
	}

	vrDetails, err := vangogh_local_data.NewReader(vangogh_local_data.Details)
	if err != nil {
		return cumu.EndWithError(err)
	}

	allDetails := vrDetails.Keys()
	unresolvedIds := make(map[string]bool)

	cumu.TotalInt(len(allDetails))
	for _, id := range allDetails {

		det, err := vrDetails.Details(id)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList, err := vangogh_local_data.FromDetails(det, rxa)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList = downloadsList.Only(operatingSystems, downloadTypes, langCodes)

		for _, dl := range downloadsList {
			if _, ok := rxa.GetFirstVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl); !ok {
				unresolvedIds[id] = true
			}
		}

		cumu.Increment()
	}

	if len(unresolvedIds) == 0 {
		cumu.EndWithResult("all good")
	} else {

		summary, err := vangogh_local_data.PropertyListsFromIdSet(
			unresolvedIds,
			nil,
			[]string{vangogh_local_data.TitleProperty},
			rxa)

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
