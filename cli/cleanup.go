package cli

import (
	"fmt"
	"math"
	"net/url"
	"os"
	"path/filepath"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

const spaceSavingsSummary = "est. disk space savings:"

func CleanupHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	q := u.Query()

	return Cleanup(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		q.Has("no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		q.Has("all"),
		q.Has("test"))
}

func Cleanup(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	all, test bool) error {

	ca := nod.NewProgress("cleaning up...")
	defer ca.Done()

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.SlugProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.ManualUrlFilenameProperty,
		vangogh_integration.ProductValidationResultProperty)
	if err != nil {
		return err
	}

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	if all {

		detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
		if err != nil {
			return err
		}

		kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
		if err != nil {
			return err
		}

		for id := range kvDetails.Keys() {
			ids = append(ids, id)
		}
	}

	cd := &cleanupDelegate{
		rdx:             rdx,
		all:             all,
		test:            test,
		downloadsLayout: downloadsLayout,
	}

	// cleaning files in local download directory that no longer map to downloads
	if err = vangogh_integration.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		cd,
		ca); err != nil {
		return err
	}

	if cd.totalBytes > 0 {
		summary := make(map[string][]string)
		summary[spaceSavingsSummary] = []string{
			fmt.Sprintf("%.2fGB", float64(cd.totalBytes)/math.Pow(1000, 3)),
		}
		ca.EndWithSummary("cleanup summary:", summary)
	}

	return nil
}

type cleanupDelegate struct {
	rdx             redux.Readable
	all             bool
	test            bool
	totalBytes      int64
	downloadsLayout vangogh_integration.DownloadsLayout
}

func (cd *cleanupDelegate) Process(id string, slug string, list vangogh_integration.DownloadsList) error {

	if err := cd.rdx.MustHave(
		vangogh_integration.ManualUrlFilenameProperty,
		vangogh_integration.ProductValidationResultProperty); err != nil {
		return err
	}

	//cleanup process:
	//0. for products that have been successfully validated
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	if pvss, ok := cd.rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
		pvs := vangogh_integration.ParseValidationStatus(pvss)
		if pvs != vangogh_integration.ValidationStatusSuccess && pvs != vangogh_integration.ValidationStatusMissingChecksum {
			// don't cleanup the product unless it's been validated, meaning we've got the latest version downloaded
			// and it passed checksum validation (or at worst is missing checksum). Don't remove (previous versions of)
			// installers for products that have validation issues
			return nil
		}
	}

	absExpectedSet := make(map[string]any)

	for _, dl := range list {

		absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.DownloadType, cd.downloadsLayout)
		if err != nil {
			return err
		}

		if filename, ok := cd.rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl); ok {
			absDownloadPath := filepath.Join(absSlugDownloadDir, filename)
			absExpectedSet[absDownloadPath] = nil
		}
	}

	//LocalSlugDownloads returns list of files relative to s/slug product directory
	absPresentSet, err := vangogh_integration.AbsLocalSlugDownloads(slug, cd.downloadsLayout)
	if err != nil {
		return err
	}

	absUnexpectedFiles := make([]string, 0, len(absPresentSet))
	for p := range absPresentSet {
		if _, ok := absExpectedSet[p]; !ok {
			absUnexpectedFiles = append(absUnexpectedFiles, p)
		}
	}

	if len(absUnexpectedFiles) == 0 {
		return nil
	}

	for _, absUnexpectedFile := range absUnexpectedFiles {
		//restore absolute from local_filename to s/slug/local_filename
		if stat, err := os.Stat(absUnexpectedFile); err == nil {
			cd.totalBytes += stat.Size()
		} else if os.IsNotExist(err) {
			continue
		} else {
			return err
		}

		prefix := ""
		if cd.test {
			prefix = "TEST"
		} else {
			prefix = "DELETE"
		}

		adp := vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Downloads)

		relDownloadFilename, err := filepath.Rel(adp, absUnexpectedFile)
		if err != nil {
			return err
		}

		dft := nod.Begin(" %s %s", prefix, relDownloadFilename)
		if !cd.test {
			if err = os.Remove(absUnexpectedFile); err != nil {
				return err
			}
		}
		dft.Done()

		absChecksumFile, err := vangogh_integration.AbsChecksumPath(absUnexpectedFile)
		if err != nil {
			return err
		}
		if stat, err := os.Stat(absChecksumFile); err == nil {
			cd.totalBytes += stat.Size()
		} else if os.IsNotExist(err) {
			continue
		} else {
			return err
		}

		acp := vangogh_integration.Pwd.AbsDirPath(vangogh_integration.Checksums)

		relChecksumFile, err := filepath.Rel(acp, absChecksumFile)
		if err != nil {
			return err
		}

		cft := nod.Begin(" %s %s", prefix, relChecksumFile)
		if !cd.test {
			if err = os.Remove(absChecksumFile); err != nil {
				return err
			}
		}
		cft.Done()
	}

	return nil
}
