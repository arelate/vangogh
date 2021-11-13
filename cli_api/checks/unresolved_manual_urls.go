package checks

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/expand"
)

func UnresolvedManualUrls(
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	fix bool) error {

	cumu := nod.NewProgress("checking unresolved manual-urls...")
	defer cumu.End()

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.LocalManualUrl)
	if err != nil {
		return cumu.EndWithError(err)
	}

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return cumu.EndWithError(err)
	}

	allDetails := vrDetails.All()
	unresolvedIds := gost.NewStrSet()

	cumu.TotalInt(len(allDetails))
	for _, id := range allDetails {

		det, err := vrDetails.Details(id)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList, err := vangogh_downloads.FromDetails(det, mt, exl)
		if err != nil {
			cumu.Error(err)
			cumu.Increment()
			continue
		}

		downloadsList = downloadsList.Only(operatingSystems, downloadTypes, langCodes)

		for _, dl := range downloadsList {
			if _, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl); !ok {
				unresolvedIds.Add(id)
			}
		}

		cumu.Increment()
	}

	if unresolvedIds.Len() == 0 {
		cumu.EndWithResult("all good")
	} else {

		heading := "found problems:"
		if fix {
			heading = "found problems (you need to run get-downloads to fix):"
		}

		summary, err := expand.IdsToPropertyLists(
			heading,
			unresolvedIds.All(),
			nil,
			[]string{vangogh_properties.TitleProperty},
			exl)

		if err != nil {
			return cumu.EndWithError(err)
		}
		cumu.EndWithSummary(summary)
	}

	return nil
}
