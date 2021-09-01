package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/iterate"
	"github.com/boggydigital/vangogh/cmd/url_helpers"
	"net/url"
	"os"
	"path/filepath"
)

const (
	dirPerm os.FileMode = 0755
)

func CleanupHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	operatingSystems := url_helpers.OperatingSystems(u)
	langCodes := url_helpers.Values(u, "language-code")
	downloadTypes := url_helpers.DownloadTypes(u)

	all := url_helpers.Flag(u, "all")

	return Cleanup(idSet, mt, operatingSystems, langCodes, downloadTypes, all)
}

func Cleanup(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_downloads.DownloadType,
	all bool) error {

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.SlugProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.LocalManualUrl)
	if err != nil {
		return err
	}

	if all {
		vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, mt)
		if err != nil {
			return err
		}
		idSet.Add(vrDetails.All()...)
	}

	if err := iterate.DownloadsList(
		idSet,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		cleanupDownloadList,
		0,
		false); err != nil {
		return err
	}

	return nil
}

func moveToRecycleBin(fp string) error {
	rbFilepath := filepath.Join(vangogh_urls.RecycleBinDir(), fp)
	rbDir, _ := filepath.Split(rbFilepath)
	if _, err := os.Stat(rbDir); os.IsNotExist(err) {
		os.MkdirAll(rbDir, dirPerm)
	}
	return os.Rename(fp, rbFilepath)
}

func cleanupDownloadList(
	id string,
	slug string,
	list vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	_ bool) error {
	fmt.Printf("cleaning up %s...", slug)

	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return err
	}

	//cleanup process:
	//1. enumerate all expected files for a downloadList
	//2. enumerate all files present for a slug (files present in a `downloads/slug` folder)
	//3. delete (present files).Except(expected files) and corresponding xml files

	expectedSet := gost.NewStrSet()

	for _, dl := range list {
		if localFilename, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl); ok {
			expectedSet.Add(localFilename)
		}
	}

	presentSet, err := vangogh_urls.LocalSlugDownloads(slug)
	if err != nil {
		return err
	}

	unexpectedFiles := presentSet.Except(expectedSet)
	if len(unexpectedFiles) == 0 {
		fmt.Println("already clean")
		return nil
	}

	for _, unexpectedFile := range unexpectedFiles {
		downloadFilename := filepath.Join(vangogh_urls.DownloadsDir(), unexpectedFile)
		if _, err := os.Stat(downloadFilename); os.IsNotExist(err) {
			continue
		}
		fmt.Print(".")
		if err := moveToRecycleBin(downloadFilename); err != nil {
			return err
		}

		checksumFile := vangogh_urls.LocalValidationPath(unexpectedFile)
		if _, err := os.Stat(checksumFile); os.IsNotExist(err) {
			fmt.Println("done")
			continue
		}
		fmt.Print("xml.")
		if err := moveToRecycleBin(checksumFile); err != nil {
			return err
		}
		fmt.Println("done")
	}

	return nil
}
