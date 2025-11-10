package itemizations

import (
	"os"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func MissingLocalDownloads(
	rdx redux.Readable,
	operatingSystems []vangogh_integration.OperatingSystem,
	downloadTypes []vangogh_integration.DownloadType,
	langCodes []string,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout) ([]string, error) {
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
		vangogh_integration.ManualUrlFilenameProperty,
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
		rdx:             rdx,
		downloadsLayout: downloadsLayout,
	}

	if err = vangogh_integration.MapDownloads(
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

	if len(mdd.missingIds) > 0 {
		mlda.EndWithResult("found missing local downloads: " + strings.Join(mdd.missingIds, ","))
	}

	return mdd.missingIds, nil
}

type missingDownloadsDelegate struct {
	rdx             redux.Readable
	missingIds      []string
	downloadsLayout vangogh_integration.DownloadsLayout
}

func (mdd *missingDownloadsDelegate) Process(id, slug string, list vangogh_integration.DownloadsList) error {

	if mdd.missingIds == nil {
		mdd.missingIds = make([]string, 0)
	}

	absExpectedFiles := make(map[string]bool)

	for _, dl := range list {

		//skip manualUrls that have produced error status codes, while they're technically missing
		//it's due to remote status for this URL, not a problem we can resolve locally
		status, ok := mdd.rdx.GetLastVal(vangogh_integration.DownloadStatusErrorProperty, dl.ManualUrl)
		if ok && status == "404" {
			continue
		}

		filename, ok := mdd.rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl)
		// 2
		if !ok || filename == "" {
			mdd.missingIds = append(mdd.missingIds, id)
			break
		}

		absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.Type, mdd.downloadsLayout)
		if err != nil {
			return err
		}

		absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

		absExpectedFiles[absDownloadPath] = true
	}

	if len(absExpectedFiles) == 0 {
		return nil
	}

	// 3
	absSlugDownloadsDir, err := vangogh_integration.AbsSlugDownloadDir(slug, vangogh_integration.Installer, mdd.downloadsLayout)
	if err != nil {
		return err
	}
	if _, err := os.Stat(absSlugDownloadsDir); os.IsNotExist(err) {
		mdd.missingIds = append(mdd.missingIds, id)
		return nil
	}

	absPresentFiles, err := vangogh_integration.AbsLocalSlugDownloads(slug, mdd.downloadsLayout)
	if err != nil {
		return nil
	}

	// 4
	absMissingFiles := make([]string, 0, len(absExpectedFiles))

	for f := range absExpectedFiles {
		if _, ok := absPresentFiles[f]; !ok {
			absMissingFiles = append(absMissingFiles, f)
		}
	}

	if len(absMissingFiles) > 0 {
		mdd.missingIds = append(mdd.missingIds, id)
	}

	return nil
}
