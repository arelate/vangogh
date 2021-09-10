package cmd

import (
	"crypto/md5"
	"encoding/xml"
	"errors"
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
	"github.com/boggydigital/vangogh/cmd/validation"
	"io"
	"net/url"
	"os"
)

var (
	ErrUnresolvedManualUrl    = errors.New("not resolved manual-url")
	ErrMissingDownload        = errors.New("download file missing")
	ErrMissingValidationFile  = errors.New("validation file missing")
	ErrValidationNotSupported = errors.New("validation not supported")
	ErrValidationFailed       = errors.New("validation failed")
)

const blockSize = 32 * 1024

func ValidateHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	operatingSystems := url_helpers.OperatingSystems(u)
	langCodes := url_helpers.Values(u, "language-code")
	downloadTypes := url_helpers.DownloadTypes(u)

	all := url_helpers.Flag(u, "all")

	return Validate(idSet, mt, operatingSystems, langCodes, downloadTypes, all)
}

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

	validated := make(map[string]bool)
	unresolvedManualUrl := make(map[string]bool)
	missingDownloads := make(map[string]bool)
	missingChecksum := make(map[string]bool)
	failed := make(map[string]bool)
	slugLastError := make(map[string]string)

	if err := iterate.DownloadsList(
		idSet,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		func(id string,
			slug string,
			list vangogh_downloads.DownloadsList,
			exl *vangogh_extracts.ExtractsList,
			_ bool) error {

			fmt.Printf("validate %s:\n", slug)

			hasValidationTargets := false

			for _, dl := range list {
				if err := validateManualUrl(slug, &dl, exl); errors.Is(err, ErrValidationNotSupported) {
					continue
				} else if errors.Is(err, ErrMissingValidationFile) {
					missingChecksum[slug] = true
				} else if errors.Is(err, ErrUnresolvedManualUrl) {
					unresolvedManualUrl[slug] = true
				} else if errors.Is(err, ErrMissingDownload) {
					missingDownloads[slug] = true
				} else if errors.Is(err, ErrValidationFailed) {
					failed[slug] = true
				} else if err != nil {
					fmt.Println(err)
					slugLastError[slug] = err.Error()
					continue
				}
				// don't attempt to assess success for files that don't support validation
				hasValidationTargets = true
			}

			if hasValidationTargets &&
				!unresolvedManualUrl[slug] &&
				!missingChecksum[slug] &&
				!failed[slug] &&
				slugLastError[slug] == "" {
				validated[slug] = true
			}

			return nil
		},
		0,
		false); err != nil {
		return err
	}

	fmt.Println()
	fmt.Println("validation summary:")
	fmt.Printf("%d product(s) successfully validated\n", len(validated))
	if len(unresolvedManualUrl) > 0 {
		fmt.Printf("%d product(s) have unresolved manual-url (not downloaded)\n", len(unresolvedManualUrl))
		for slug := range unresolvedManualUrl {
			fmt.Println("", slug)
		}
	}
	if len(missingDownloads) > 0 {
		fmt.Printf("%d product(s) missing downloads\n", len(missingDownloads))
		for slug := range missingDownloads {
			fmt.Println("", slug)
		}
	}
	if len(missingChecksum) > 0 {
		fmt.Printf("%d product(s) without checksum:\n", len(missingChecksum))
		for slug := range missingChecksum {
			fmt.Println("", slug)
		}
	}
	if len(failed) > 0 {
		fmt.Printf("%d product(s) failed validation:\n", len(failed))
		for slug := range failed {
			fmt.Println("", slug)
		}
	}
	if len(slugLastError) > 0 {
		fmt.Printf("%d product(s) validation caused an error:\n", len(slugLastError))
		for slug, err := range slugLastError {
			fmt.Printf(" %s: %s\n", slug, err)
		}
	}

	return nil
}

type progress struct {
	total   uint64
	current uint64
	notify  func(uint64, uint64)
}

func (pr *progress) Write(p []byte) (int, error) {
	n := len(p)
	pr.current += uint64(n)
	if pr.notify != nil {
		pr.notify(pr.current, pr.total)
	}
	return n, nil
}

func validateManualUrl(
	slug string,
	dl *vangogh_downloads.Download,
	exl *vangogh_extracts.ExtractsList) error {

	if err := exl.AssertSupport(vangogh_properties.LocalManualUrl); err != nil {
		return err
	}

	//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
	localFile, ok := exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl)
	if !ok {
		return ErrUnresolvedManualUrl
	}

	//absolute path (given a downloads/ root) for a s/slug/local_filename,
	//e.g. downloads/s/slug/local_filename
	absLocalFile := vangogh_urls.DownloadRelToAbs(localFile)
	if !vangogh_urls.CanValidate(absLocalFile) {
		return ErrValidationNotSupported
	}

	if _, err := os.Stat(absLocalFile); os.IsNotExist(err) {
		return ErrMissingDownload
	}

	absChecksumFile := vangogh_urls.LocalChecksumPath(absLocalFile)

	if _, err := os.Stat(absChecksumFile); os.IsNotExist(err) {
		return ErrMissingValidationFile
	}

	fmt.Printf(" %s", dl)

	chkFile, err := os.Open(absChecksumFile)
	if err != nil {
		return err
	}
	defer chkFile.Close()

	var chkData validation.File
	if err := xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return err
	}

	sourceFile, err := os.Open(absLocalFile)
	if err != nil {
		return err
	}
	defer sourceFile.Close()

	h := md5.New()

	stat, err := sourceFile.Stat()
	if err != nil {
		return err
	}

	prg := &progress{
		total: uint64(stat.Size())}

	prg.notify = printCompletion
	prg.notify(0, uint64(stat.Size()))

	for {
		_, err = io.CopyN(h, io.TeeReader(sourceFile, prg), blockSize)
		if err == io.EOF {
			// This is not an error in the common sense
			// io.EOF tells us, that we did read the complete body
			break
		} else if err != nil {
			//You should do error handling here
			return err
		}
	}
	prg.notify(uint64(stat.Size()), uint64(stat.Size()))

	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if chkData.MD5 == sourceFileMD5 {
		//spacing is intentional here to overwrite 100% progress (so it has to be 4 characters)
		fmt.Println(" ok ")
	} else {
		fmt.Println(" FAIL")
		return ErrValidationFailed
	}

	return nil
}
