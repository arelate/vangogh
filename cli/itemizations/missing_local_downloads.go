package itemizations

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
)

func MissingLocalDownloads(
	rdx kvas.ReadableRedux,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string) (map[string]bool, error) {
	//enumerating missing local downloads is a bit more complicated than images and videos
	//due to the fact that actual filenames are resolved when downloads are processed, so we can't compare
	//manualUrls and available files, we need to resolve manualUrls to actual local filenames first.
	//with this in mind we'll use different approach:
	//1. for all vangogh_local_data.Details ids:
	//2. check if there are unresolved manualUrls -> add to missingIds
	//3. check if slug dir is not present in downloads -> add to missingIds
	//4. check if any expected (resolved manualUrls) files are not present -> add to missingIds

	mlda := nod.NewProgress(" itemizing missing local downloads")
	defer mlda.End()

	emptySet := make(map[string]bool)

	if err := rdx.MustHave(
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.DownloadStatusErrorProperty); err != nil {
		return emptySet, mlda.EndWithError(err)
	}

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return emptySet, mlda.EndWithError(err)
	}

	//1
	allIds := make(map[string]bool)
	for _, id := range vrDetails.Keys() {
		allIds[id] = true
	}

	mlda.TotalInt(len(allIds))

	mdd := &missingDownloadsDelegate{
		rdx: rdx}

	if err := vangogh_local_data.MapDownloads(
		allIds,
		rdx,
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
	rdx        kvas.ReadableRedux
	missingIds map[string]bool
}

func (mdd *missingDownloadsDelegate) Process(id, slug string, list vangogh_local_data.DownloadsList) error {

	if mdd.missingIds == nil {
		mdd.missingIds = make(map[string]bool)
	}

	//pDir = s/slug
	relDir, err := vangogh_local_data.RelProductDownloadsDir(slug)
	if err != nil {
		return err
	}

	expectedFiles := make(map[string]bool)

	for _, dl := range list {

		//skip manualUrls that have produced error status codes, while they're technically missing
		//it's due to remote status for this URL, not a problem we can resolve locally
		status, ok := mdd.rdx.GetFirstVal(vangogh_local_data.DownloadStatusErrorProperty, dl.ManualUrl)
		if ok && status == "404" {
			continue
		}

		localFilename, ok := mdd.rdx.GetFirstVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl)
		// 2
		if !ok || localFilename == "" {
			mdd.missingIds[id] = true
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
	absDir, err := vangogh_local_data.AbsProductDownloadsDir(slug)
	if err != nil {
		return err
	}
	if _, err := os.Stat(absDir); os.IsNotExist(err) {
		mdd.missingIds[id] = true
		return nil
	}

	presentFiles, err := vangogh_local_data.LocalSlugDownloads(slug)
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
		mdd.missingIds[id] = true
	}

	return nil
}
