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
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
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

	mlda := nod.NewProgress(" itemizing missing local downloads")
	defer mlda.End()

	if err := exl.AssertSupport(
		vangogh_properties.LocalManualUrl,
		vangogh_properties.DownloadStatusError); err != nil {
		return nil, mlda.EndWithError(err)
	}

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
	if err != nil {
		return nil, mlda.EndWithError(err)
	}

	//1
	allIds := gost.NewStrSetWith(vrDetails.All()...)

	mlda.TotalInt(allIds.Len())

	mdd := &missingDownloadsDelegate{
		exl: exl}

	if err := vangogh_downloads.Map(
		allIds,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		mdd,
		mlda); err != nil {
		return mdd.missingIds, mlda.EndWithError(err)
	}

	return mdd.missingIds, nil
}

type missingDownloadsDelegate struct {
	exl        *vangogh_extracts.ExtractsList
	missingIds gost.StrSet
}

func (mdd *missingDownloadsDelegate) Process(id, slug string, list vangogh_downloads.DownloadsList) error {

	if mdd.missingIds == nil {
		mdd.missingIds = gost.NewStrSet()
	}

	//pDir = s/slug
	relDir, err := vangogh_urls.ProductDownloadsRelDir(slug)
	if err != nil {
		return err
	}

	expectedFiles := gost.NewStrSet()

	for _, dl := range list {

		//skip manualUrls that have produced error status codes, while they're technically missing
		//it's due to remote status for this URL, not a problem we can resolve locally
		status, ok := mdd.exl.Get(vangogh_properties.DownloadStatusError, dl.ManualUrl)
		if ok && status == "404" {
			continue
		}

		localFilename, ok := mdd.exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
		// 2
		if !ok || localFilename == "" {
			mdd.missingIds.Add(id)
			break
		}
		//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
		//so filepath.Rel would trim to local_filename (or dlc/local_filename, extra/local_filename)
		relFilename, err := filepath.Rel(relDir, localFilename)
		if err != nil {
			return err
		}

		expectedFiles.Add(relFilename)
	}

	if expectedFiles.Len() == 0 {
		return nil
	}

	// 3
	absDir, err := vangogh_urls.ProductDownloadsAbsDir(slug)
	if err != nil {
		return err
	}
	if _, err := os.Stat(absDir); os.IsNotExist(err) {
		mdd.missingIds.Add(id)
		return nil
	}

	presentFiles, err := vangogh_urls.LocalSlugDownloads(slug)
	if err != nil {
		return nil
	}

	// 4
	missingFiles := expectedFiles.Except(presentFiles)
	if len(missingFiles) > 0 {
		mdd.missingIds.Add(id)
	}

	return nil
}
