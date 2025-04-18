package cli

import (
	"errors"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"math"
	"net/url"
	"os"
	"path/filepath"
)

const spaceSavingsSummary = "est. disk space savings:"

func CleanupHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Cleanup(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "all"),
		vangogh_integration.FlagFromUrl(u, "test"),
		vangogh_integration.FlagFromUrl(u, "delete"))
}

func Cleanup(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	all, test, delete bool) error {

	ca := nod.NewProgress("cleaning up...")
	defer ca.Done()

	if test && delete {
		return errors.New("cleanup can be either test or delete, not both at the same time")
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir,
		vangogh_integration.SlugProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.ManualUrlFilenameProperty)
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
		delete:          delete,
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
	delete          bool
	totalBytes      int64
	downloadsLayout vangogh_integration.DownloadsLayout
}

func (cd *cleanupDelegate) Process(_ string, slug string, list vangogh_integration.DownloadsList) error {

	csa := nod.QueueBegin(slug)
	defer csa.Done()

	if err := cd.rdx.MustHave(vangogh_integration.ManualUrlFilenameProperty); err != nil {
		return err
	}

	//cleanup process:
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	absExpectedSet := make(map[string]any)

	for _, dl := range list {

		absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.Type, cd.downloadsLayout)
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
		if !cd.all {
			csa.EndWithResult("already clean")
			csa.Flush()
		}
		return nil
	}

	//given some unexpected files - flush message queue to output slug and put the files
	//output next in context of a slug we've queued earlier
	csa.Flush()

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
			if cd.delete {
				prefix = "DELETE"
			} else {
				prefix = "RECYCLE"
			}
		}

		adp, err := pathways.GetAbsDir(vangogh_integration.Downloads)
		if err != nil {
			return err
		}

		relDownloadFilename, err := filepath.Rel(adp, absUnexpectedFile)
		if err != nil {
			return err
		}

		dft := nod.Begin(" %s %s", prefix, relDownloadFilename)
		if !cd.test {
			if cd.delete {
				if err := os.Remove(absUnexpectedFile); err != nil {
					return err
				}
			} else {
				if err := vangogh_integration.MoveToRecycleBin(adp, absUnexpectedFile); err != nil {
					return err
				}
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

		acp, err := pathways.GetAbsDir(vangogh_integration.Checksums)
		if err != nil {
			return err
		}

		relChecksumFile, err := filepath.Rel(acp, absChecksumFile)
		if err != nil {
			return err
		}

		cft := nod.Begin(" %s %s", prefix, relChecksumFile)
		if !cd.test {
			if err := vangogh_integration.MoveToRecycleBin(acp, absChecksumFile); err != nil {
				return err
			}
		}
		cft.Done()
	}

	return nil
}
