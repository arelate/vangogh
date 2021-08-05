package cmd

import (
	"crypto/md5"
	"encoding/xml"
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
	"github.com/boggydigital/vangogh/cmd/validation"
	"io"
	"os"
	"path"
)

func Validate(
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
		validateDownloadList,
		0,
		false); err != nil {
		return err
	}

	return nil
}

func validateDownloadList(
	slug string,
	list vangogh_downloads.DownloadsList,
	exl *vangogh_extracts.ExtractsList,
	_ bool) error {
	fmt.Println("validating", slug)

	for _, dl := range list {
		if err := validateManualUrl(slug, &dl, exl); err != nil {
			return err
		}
	}

	return nil
}

func validateManualUrl(
	slug string,
	dl *vangogh_downloads.Download,
	exl *vangogh_extracts.ExtractsList) error {

	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return err
	}

	relLocalFile, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
	if !ok {
		fmt.Printf("vangogh: %s file for %s have not been successfully downloaded yet\n", slug, dl.ManualUrl)
		return nil
	}

	absLocalFile := path.Join(vangogh_urls.DownloadsDir(), relLocalFile)
	if !vangogh_urls.CanValidate(absLocalFile) {
		return nil
	}

	if _, err := os.Stat(absLocalFile); os.IsNotExist(err) {
		fmt.Printf("vangogh: %s local file %s does not exist\n", slug, absLocalFile)
		return nil
	}

	absValidationFile := vangogh_urls.LocalValidationPath(absLocalFile)

	if _, err := os.Stat(absValidationFile); os.IsNotExist(err) {
		fmt.Printf("vangogh: %s validation file %s does not exist\n", slug, absValidationFile)
		return nil
	}

	fmt.Printf("validing %s...", dl)

	valFile, err := os.Open(absValidationFile)
	if err != nil {
		return err
	}
	defer valFile.Close()

	var valData validation.File
	if err := xml.NewDecoder(valFile).Decode(&valData); err != nil {
		return err
	}

	sourceFile, err := os.Open(absLocalFile)
	if err != nil {
		return err
	}
	defer sourceFile.Close()

	h := md5.New()
	if _, err := io.Copy(h, sourceFile); err != nil {
		return err
	}
	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if valData.MD5 == sourceFileMD5 {
		fmt.Println("ok")
	} else {
		fmt.Println("FAIL")
	}

	return nil
}
