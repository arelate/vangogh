package cli_api

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
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
	"os"
	"path/filepath"
)

func CleanupHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	operatingSystems := url_helpers.OperatingSystems(u)
	downloadTypes := url_helpers.DownloadTypes(u)
	langCodes := url_helpers.Values(u, "language-code")

	all := url_helpers.Flag(u, "all")
	test := url_helpers.Flag(u, "test")

	return Cleanup(idSet, mt, operatingSystems, downloadTypes, langCodes, all, test)
}

func Cleanup(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	all, test bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.SlugProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.LocalManualUrl)
	if err != nil {
		return err
	}

	ca := nod.NewProgress("cleaning up:")
	defer ca.End()

	if all {
		vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
		if err != nil {
			return err
		}
		idSet.Add(vrDetails.All()...)
	}

	cd := &cleanupDelegate{
		exl:  exl,
		all:  all,
		test: test,
		tpw:  ca,
	}

	ca.Total(uint64(idSet.Len()))

	if err := vangogh_downloads.Map(
		idSet,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		cd); err != nil {
		return err
	}

	return nil
}

func moveToRecycleBin(fp string) error {
	rbFilepath := filepath.Join(vangogh_urls.RecycleBinDir(), fp)
	rbDir, _ := filepath.Split(rbFilepath)
	if _, err := os.Stat(rbDir); os.IsNotExist(err) {
		if err := os.MkdirAll(rbDir, 0755); err != nil {
			return err
		}
	}
	return os.Rename(fp, rbFilepath)
}

type cleanupDelegate struct {
	exl  *vangogh_extracts.ExtractsList
	all  bool
	test bool
	tpw  nod.TotalProgressWriter
}

func (cd *cleanupDelegate) Process(_ string, slug string, list vangogh_downloads.DownloadsList) error {

	csa := nod.QueueBegin(slug)
	defer csa.End()
	defer cd.tpw.Increment()

	if err := cd.exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return err
	}

	//cleanup process:
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	expectedSet := gost.NewStrSet()

	//pDir = s/slug
	pDir, err := vangogh_urls.ProductDownloadsRelDir(slug)
	if err != nil {
		return err
	}

	for _, dl := range list {
		if localFilename, ok := cd.exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl); ok {
			//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
			//so filepath.Rel would trim to local_filename (or dlc/local_filename, extra/local_filename)
			relFilename, err := filepath.Rel(pDir, localFilename)
			if err != nil {
				return err
			}
			expectedSet.Add(relFilename)
		}
	}

	//LocalSlugDownloads returns list of files relative to s/slug product directory
	presentSet, err := vangogh_urls.LocalSlugDownloads(slug)
	if err != nil {
		return err
	}

	unexpectedFiles := presentSet.Except(expectedSet)

	if len(unexpectedFiles) == 0 {
		if !cd.all {
			csa.EndWithResult("- already clean")
			csa.Flush()
		}
		return nil
	}

	//given some unexpected files - flush message queue to output slug and put the files
	//output next in context of a slug we've queued earlier
	csa.Flush()

	for _, unexpectedFile := range unexpectedFiles {
		//restore absolute from local_filename to s/slug/local_filename
		downloadFilename := vangogh_urls.DownloadRelToAbs(filepath.Join(pDir, unexpectedFile))
		if _, err := os.Stat(downloadFilename); os.IsNotExist(err) {
			continue
		}
		prefix := "DELETE"
		if cd.test {
			prefix = "TEST"
		}

		dft := nod.Begin(" %s %s", prefix, downloadFilename)
		if !cd.test {
			if err := moveToRecycleBin(downloadFilename); err != nil {
				return dft.EndWithError(err)
			}
		}
		dft.End()

		checksumFile := vangogh_urls.LocalChecksumPath(downloadFilename)
		if _, err := os.Stat(checksumFile); os.IsNotExist(err) {
			continue
		}

		cft := nod.Begin(" %s %s", prefix, checksumFile)
		if !cd.test {
			if err := moveToRecycleBin(checksumFile); err != nil {
				return cft.EndWithError(err)
			}
		}
		cft.End()
	}

	return nil
}
