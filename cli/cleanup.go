package cli

import (
	"errors"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"math"
	"net/url"
	"os"
	"path/filepath"
)

const spaceSavingsSummary = "est. disk space savings:"

func CleanupHandler(u *url.URL) error {
	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Cleanup(
		ids,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		vangogh_local_data.FlagFromUrl(u, "all"),
		vangogh_local_data.FlagFromUrl(u, "test"),
		vangogh_local_data.FlagFromUrl(u, "delete"))
}

func Cleanup(
	ids []string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	all, test, delete bool) error {

	if test && delete {
		return errors.New("cleanup can be either test or delete, not both at the same time")
	}

	rdx, err := vangogh_local_data.NewReduxReader(
		vangogh_local_data.SlugProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.LocalManualUrlProperty)
	if err != nil {
		return err
	}

	ca := nod.NewProgress("cleaning up...")
	defer ca.End()

	vangogh_local_data.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	if all {
		vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
		if err != nil {
			return err
		}
		keys, err := vrDetails.Keys()
		if err != nil {
			return ca.EndWithError(err)
		}
		ids = append(ids, keys...)
	}

	cd := &cleanupDelegate{
		rdx:    rdx,
		all:    all,
		test:   test,
		delete: delete,
	}

	// cleaning files in local download directory that no longer map to downloads
	if err := vangogh_local_data.MapDownloads(
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
	} else {
		ca.EndWithResult("done")
	}

	return nil
}

type cleanupDelegate struct {
	rdx        kevlar.ReadableRedux
	all        bool
	test       bool
	delete     bool
	totalBytes int64
}

func (cd *cleanupDelegate) Process(_ string, slug string, list vangogh_local_data.DownloadsList) error {

	csa := nod.QueueBegin(slug)
	defer csa.End()

	if err := cd.rdx.MustHave(vangogh_local_data.LocalManualUrlProperty); err != nil {
		return csa.EndWithError(err)
	}

	//cleanup process:
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	expectedSet := make(map[string]bool)

	//pDir = s/slug
	pDir, err := vangogh_local_data.RelProductDownloadsDir(slug)
	if err != nil {
		return csa.EndWithError(err)
	}

	for _, dl := range list {
		if localFilename, ok := cd.rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl); ok {
			//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
			//so filepath.Rel would trim to local_filename (or dlc/local_filename, extra/local_filename)
			relFilename, err := filepath.Rel(pDir, localFilename)
			if err != nil {
				return csa.EndWithError(err)
			}
			expectedSet[relFilename] = true
		}
	}

	//LocalSlugDownloads returns list of files relative to s/slug product directory
	presentSet, err := vangogh_local_data.LocalSlugDownloads(slug)
	if err != nil {
		return csa.EndWithError(err)
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
		absDownloadFilename, err := vangogh_local_data.AbsDownloadDirFromRel(filepath.Join(pDir, unexpectedFile))
		if err != nil {
			return csa.EndWithError(err)
		}
		if stat, err := os.Stat(absDownloadFilename); err == nil {
			cd.totalBytes += stat.Size()
		} else if os.IsNotExist(err) {
			continue
		} else {
			return csa.EndWithError(err)
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

		adp, err := pathways.GetAbsDir(vangogh_local_data.Downloads)
		if err != nil {
			return csa.EndWithError(err)
		}

		relDownloadFilename, err := filepath.Rel(adp, absDownloadFilename)
		if err != nil {
			return csa.EndWithError(err)
		}

		dft := nod.Begin(" %s %s", prefix, relDownloadFilename)
		if !cd.test {
			if cd.delete {
				if err := os.Remove(absDownloadFilename); err != nil {
					return dft.EndWithError(err)
				}
			} else {
				if err := vangogh_local_data.MoveToRecycleBin(adp, absDownloadFilename); err != nil {
					return dft.EndWithError(err)
				}
			}
		}
		dft.End()

		absChecksumFile, err := vangogh_local_data.AbsLocalChecksumPath(absDownloadFilename)
		if err != nil {
			return csa.EndWithError(err)
		}
		if stat, err := os.Stat(absChecksumFile); err == nil {
			cd.totalBytes += stat.Size()
		} else if os.IsNotExist(err) {
			continue
		} else {
			return csa.EndWithError(err)
		}

		acp, err := pathways.GetAbsDir(vangogh_local_data.Checksums)
		if err != nil {
			return csa.EndWithError(err)
		}

		relChecksumFile, err := filepath.Rel(acp, absChecksumFile)
		if err != nil {
			return csa.EndWithError(err)
		}

		cft := nod.Begin(" %s %s", prefix, relChecksumFile)
		if !cd.test {
			if err := vangogh_local_data.MoveToRecycleBin(acp, absChecksumFile); err != nil {
				return cft.EndWithError(err)
			}
		}
		cft.End()
	}

	return nil
}
