package vets

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"slices"
)

func UnresolvedManualUrls(
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	fix bool) error {

	cumu := nod.NewProgress("checking unresolved manual-urls...")
	defer cumu.Done()

	vangogh_integration.PrintParams(nil, operatingSystems, langCodes, downloadTypes, noPatches)

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir,
		vangogh_integration.TitleProperty,
		vangogh_integration.LocalManualUrlProperty)
	if err != nil {
		return err
	}

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	unresolvedIds := make(map[string]bool)

	cumu.TotalInt(kvDetails.Len())

	for id := range kvDetails.Keys() {

		det, err := vangogh_integration.UnmarshalDetails(id, kvDetails)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		if det == nil {
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
			slices.Collect(maps.Keys(unresolvedIds)),
			nil,
			[]string{vangogh_integration.TitleProperty},
			rdx)

		heading := fmt.Sprintf("found %d problems:", len(unresolvedIds))
		if fix {
			heading = "found problems (run get-downloads to fix):"
		}

		if err != nil {
			return err
		}
		cumu.EndWithSummary(heading, summary)
	}

	return nil
}
