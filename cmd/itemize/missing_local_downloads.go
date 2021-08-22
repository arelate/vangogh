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

	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return nil, err
	}

	missingIds := gost.NewStrSet()

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return nil, err
	}

	// 1
	for _, id := range vrDetails.All() {

		det, err := vrDetails.Details(id)
		if err != nil {
			return missingIds, err
		}

		downloads, err := vangogh_downloads.FromDetails(det, mt, exl)
		if err != nil {
			return missingIds, err
		}

		downloads = downloads.Only(operatingSystems, downloadTypes, langCodes)

		expectedFiles := gost.NewStrSet()
		for _, dl := range downloads {
			file, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
			// 2
			if !ok || file == "" {
				missingIds.Add(id)
				break
			}
			expectedFiles.Add(file)
		}

		if expectedFiles.Len() == 0 {
			continue
		}

		slug, ok := exl.Get(vangogh_properties.SlugProperty, id)
		if !ok {
			continue
		}

		// 3
		if _, err := os.Stat(vangogh_urls.ProductDownloadsDir(slug)); os.IsNotExist(err) {
			missingIds.Add(id)
			continue
		}

		presentFiles, err := vangogh_urls.LocalSlugDownloads(slug)
		if err != nil {
			return missingIds, nil
		}

		// 4
		missingFiles := expectedFiles.Except(presentFiles)
		if len(missingFiles) > 0 {
			missingIds.Add(id)
		}
	}

	return missingIds, nil
}
