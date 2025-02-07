package vets

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func UnresolvedManualUrls(
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	fix bool) error {

	cumu := nod.NewProgress("checking unresolved manual-urls...")
	defer cumu.End()

	vangogh_integration.PrintParams(nil, operatingSystems, langCodes, downloadTypes, noPatches)

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.TitleProperty,
		//vangogh_integration.NativeLanguageNameProperty,
		vangogh_integration.LocalManualUrlProperty)
	if err != nil {
		return cumu.EndWithError(err)
	}

	vrDetails, err := vangogh_integration.NewProductReader(vangogh_integration.Details)
	if err != nil {
		return cumu.EndWithError(err)
	}

	unresolvedIds := make(map[string]bool)

	cumu.TotalInt(vrDetails.Len())

	for id := range vrDetails.Keys() {

		det, err := vrDetails.Details(id)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList, err := vangogh_integration.FromDetails(det, rdx)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList = downloadsList.Only(operatingSystems, langCodes, downloadTypes, noPatches)

		for _, dl := range downloadsList {
			if _, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl); !ok {
				unresolvedIds[id] = true
			}
		}

		cumu.Increment()
	}

	if len(unresolvedIds) == 0 {
		cumu.EndWithResult("all good")
	} else {

		summary, err := vangogh_integration.PropertyListsFromIdSet(
			maps.Keys(unresolvedIds),
			nil,
			[]string{vangogh_integration.TitleProperty},
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
