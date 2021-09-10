package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/iterate"
	"os"
)

func MissingLocalDownloads(
	mt gog_media.Media,
	exl *vangogh_extracts.ExtractsList,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string) (gost.StrSet, error) {
	//enumerating missing local downloads is a bit more complicated than images and videos
	//due to the fact that actual filenames are resolved when downloads are processed, so we can't compare
	//manualUrls and available files, we need to resolve manualUrls to actual local filenames first.
	//with this in mind we'll use different approach:
	//1. for all vangogh_products.Details ids:
	//2. check if there are unresolved manualUrls -> add to missingIds
	//3. check if slug dir is not present in downloads -> add to missingIds
	//4. check if any expected (resolved manualUrls) files are not present -> add to missingIds

	if err := exl.AssertSupport(
		vangogh_properties.LocalManualUrl,
		vangogh_properties.DownloadStatusError); err != nil {
		return nil, err
	}

	missingIds := gost.NewStrSet()

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return nil, err
	}

	//1
	allIds := gost.NewStrSetWith(vrDetails.All()...)

	iterate.DownloadsList(
		allIds,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		func(id string,
			slug string,
			dlList vangogh_downloads.DownloadsList,
			exl *vangogh_extracts.ExtractsList,
			forceRemoteUpdate bool) error {

			expectedFiles := gost.NewStrSet()

			for _, dl := range dlList {

				//skip manualUrls that have produced error status codes, while they're technically missing
				//it's due to remote status for this URL, not a problem we can resolve locally
				status, ok := exl.Get(vangogh_properties.DownloadStatusError, dl.ManualUrl)
				if ok && status == "404" {
					continue
				}

				file, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
				// 2
				if !ok || file == "" {
					missingIds.Add(id)
					break
				}
				expectedFiles.Add(file)
			}

			if expectedFiles.Len() == 0 {
				return nil
			}

			// 3
			pDir, err := vangogh_urls.ProductDownloadsAbsDir(slug)
			if err != nil {
				return err
			}
			if _, err := os.Stat(pDir); os.IsNotExist(err) {
				missingIds.Add(id)
				return nil
			}

			presentFiles, err := vangogh_urls.LocalSlugDownloads(slug)
			if err != nil {
				return nil
			}

			// 4
			missingFiles := expectedFiles.Except(presentFiles)
			if len(missingFiles) > 0 {
				missingIds.Add(id)
			}

			return nil
		},
		0,
		false,
	)

	return missingIds, nil
}
