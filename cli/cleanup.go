package cli

import (
	"errors"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
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
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
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
	all, test, delete bool) error {

	if test && delete {
		return errors.New("cleanup can be either test or delete, not both at the same time")
	}

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.SlugProperty,
		//vangogh_integration.NativeLanguageNameProperty,
		vangogh_integration.LocalManualUrlProperty)
	if err != nil {
		return err
	}

	ca := nod.NewProgress("cleaning up...")
	defer ca.EndWithResult("done")

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	if all {
		vrDetails, err := vangogh_integration.NewProductReader(vangogh_integration.Details)
		if err != nil {
			return err
		}
		for id := range vrDetails.Keys() {
			ids = append(ids, id)
		}
	}

	cd := &cleanupDelegate{
		rdx:    rdx,
		all:    all,
		test:   test,
		delete: delete,
	}

	// cleaning files in local download directory that no longer map to downloads
	if err := vangogh_integration.MapDownloads(
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
	rdx        redux.Readable
	all        bool
	test       bool
	delete     bool
	totalBytes int64
}

func (cd *cleanupDelegate) Process(_ string, slug string, list vangogh_integration.DownloadsList) error {

	csa := nod.QueueBegin(slug)
	defer csa.EndWithResult("done")

	if err := cd.rdx.MustHave(vangogh_integration.LocalManualUrlProperty); err != nil {
		return err
	}

	//cleanup process:
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	expectedSet := make(map[string]bool)

	//pDir = s/slug
	pDir, err := vangogh_integration.RelProductDownloadsDir(slug)
	if err != nil {
		return err
	}

	for _, dl := range list {
		if localFilename, ok := cd.rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl); ok {
			//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
			//so filepath.Rel would trim to local_filename (or dlc/local_filename, extra/local_filename)
			relFilename, err := filepath.Rel(pDir, localFilename)
			if err != nil {
				return err
			}
			expectedSet[relFilename] = true
		}
	}

	//LocalSlugDownloads returns list of files relative to s/slug product directory
	presentSet, err := vangogh_integration.LocalSlugDownloads(slug)
	if err != nil {
		return err
	}

	unexpectedFiles := make([]string, 0, len(presentSet))
	for p := range presentSet {
		if !expectedSet[p] {
			unexpectedFiles = append(unexpectedFiles, p)
		}
	}

	if len(unexpectedFiles) == 0 {
		if !cd.all {
			csa.EndWithResult("already clean")
			csa.Flush()
		}
		return nil
	}

	//given some unexpected files - flush message queue to output slug and put the files
	//output next in context of a slug we've queued earlier
	csa.Flush()

	for _, unexpectedFile := range unexpectedFiles {
		//restore absolute from local_filename to s/slug/local_filename
		absDownloadFilename, err := vangogh_integration.AbsDownloadDirFromRel(filepath.Join(pDir, unexpectedFile))
		if err != nil {
			return err
		}
		if stat, err := os.Stat(absDownloadFilename); err == nil {
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

		relDownloadFilename, err := filepath.Rel(adp, absDownloadFilename)
		if err != nil {
			return err
		}

		dft := nod.Begin(" %s %s", prefix, relDownloadFilename)
		if !cd.test {
			if cd.delete {
				if err := os.Remove(absDownloadFilename); err != nil {
					return err
				}
			} else {
				if err := vangogh_integration.MoveToRecycleBin(adp, absDownloadFilename); err != nil {
					return err
				}
			}
		}
		dft.EndWithResult("done")

		absChecksumFile, err := vangogh_integration.AbsLocalChecksumPath(absDownloadFilename)
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
		cft.EndWithResult("done")
	}

	return nil
}
