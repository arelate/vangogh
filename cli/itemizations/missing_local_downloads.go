package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"os"
	"path/filepath"
)

func MissingLocalDownloads(
	rdx redux.Readable,
	operatingSystems []vangogh_integration.OperatingSystem,
	downloadTypes []vangogh_integration.DownloadType,
	langCodes []string,
	noPatches bool) ([]string, error) {
	//enumerating missing local downloads is a bit more complicated than images
	//due to the fact that actual filenames are resolved when downloads are processed, so we can't compare
	//manualUrls and available files, we need to resolve manualUrls to actual local filenames first.
	//with this in mind we'll use different approach:
	//1. for all vangogh_integration.Details ids:
	//2. check if there are unresolved manualUrls -> add to missingIds
	//3. check if slug dir is not present in downloads -> add to missingIds
	//4. check if any expected (resolved manualUrls) files are not present -> add to missingIds

	mlda := nod.NewProgress(" itemizing missing local downloads")
	defer mlda.Done()

	if err := rdx.MustHave(
		vangogh_integration.LocalManualUrlProperty,
		vangogh_integration.DownloadStatusErrorProperty); err != nil {
		return nil, err
	}

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return nil, err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	//1
	var allIds []string
	for id := range kvDetails.Keys() {
		allIds = append(allIds, id)
	}

	mlda.TotalInt(len(allIds))

	mdd := &missingDownloadsDelegate{
		rdx: rdx}

	if err := vangogh_integration.MapDownloads(
		allIds,
		rdx,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		mdd,
		mlda); err != nil {
		return mdd.missingIds, err
	}

	return mdd.missingIds, nil
}

type missingDownloadsDelegate struct {
	rdx        redux.Readable
	missingIds []string
}

func (mdd *missingDownloadsDelegate) Process(id, slug string, list vangogh_integration.DownloadsList) error {

	if mdd.missingIds == nil {
		mdd.missingIds = make([]string, 0)
	}

	//pDir = s/slug
	relDir, err := vangogh_integration.RelProductDownloadsDir(slug)
	if err != nil {
		return err
	}

	expectedFiles := make(map[string]bool)

	for _, dl := range list {

		//skip manualUrls that have produced error status codes, while they're technically missing
		//it's due to remote status for this URL, not a problem we can resolve locally
		status, ok := mdd.rdx.GetLastVal(vangogh_integration.DownloadStatusErrorProperty, dl.ManualUrl)
		if ok && status == "404" {
			continue
		}

		localFilename, ok := mdd.rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl)
		// 2
		if !ok || localFilename == "" {
			mdd.missingIds = append(mdd.missingIds, id)
			break
		}
		//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
		//so filepath.Rel would trim to local_filename (or dlc/local_filename, extra/local_filename)
		relFilename, err := filepath.Rel(relDir, localFilename)
		if err != nil {
			return err
		}

		expectedFiles[relFilename] = true
	}

	if len(expectedFiles) == 0 {
		return nil
	}

	// 3
	absDir, err := vangogh_integration.AbsProductDownloadsDir(slug)
	if err != nil {
		return err
	}
	if _, err := os.Stat(absDir); os.IsNotExist(err) {
		mdd.missingIds = append(mdd.missingIds, id)
		return nil
	}

	presentFiles, err := vangogh_integration.LocalSlugDownloads(slug)
	if err != nil {
		return nil
	}

	// 4
	missingFiles := make([]string, 0, len(expectedFiles))

	for f := range expectedFiles {
		if !presentFiles[f] {
			missingFiles = append(missingFiles, f)
		}
	}

	if len(missingFiles) > 0 {
		mdd.missingIds = append(mdd.missingIds, id)
	}

	return nil
}
