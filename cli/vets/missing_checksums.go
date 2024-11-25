package vets

import (
	"crypto/md5"
	"encoding/xml"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
	"strings"
)

func MissingChecksums(fix bool) error {

	mca := nod.Begin("looking for missing checksums...")
	defer mca.End()

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.ManualUrlStatusProperty,
		vangogh_local_data.ManualUrlGeneratedChecksumProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.ProductTypeProperty)
	if err != nil {
		return mca.EndWithError(err)
	}

	idManualUrls := make(map[string][]string)

	for _, id := range rdx.Keys(vangogh_local_data.ManualUrlStatusProperty) {

		// skip DLC and PACK product types as they don't have Details for their ids and would
		// crash below attempting to read vrDetails for missing product. That's totally ok, since
		// DLC and PACK product types get validation status from cascading, so as we fix GAME
		// product types and perform cascade - we'll eventually get correct status for all cascaded types
		if pt, ok := rdx.GetLastVal(vangogh_local_data.ProductTypeProperty, id); ok && pt != "GAME" {
			continue
		}

		if muses, ok := rdx.GetAllValues(vangogh_local_data.ManualUrlStatusProperty, id); ok {
			for _, mus := range muses {
				if mu, str, ok := strings.Cut(mus, "="); ok {
					if status := vangogh_local_data.ParseManualUrlStatus(str); status == vangogh_local_data.ManualUrlNotValidatedMissingChecksum {
						idManualUrls[id] = append(idManualUrls[id], mu)
					}
				}
			}
		}
	}

	filesMissingChecksums := make(map[string]interface{})

	for id, manualUrls := range idManualUrls {

		for _, mu := range manualUrls {
			relFile, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, mu)
			if !ok {
				continue
			}

			absChecksumFile, err := vangogh_local_data.AbsLocalChecksumPath(relFile)
			if err != nil {
				return mca.EndWithError(err)
			}
			if _, err := os.Stat(absChecksumFile); err == nil {
				continue
			}

			if fix {

				if err := generateChecksumForFile(id, mu, relFile); err != nil {
					return mca.EndWithError(err)
				}

				if err := rdx.AddValues(vangogh_local_data.ManualUrlGeneratedChecksumProperty, id, mu); err != nil {
					return mca.EndWithError(err)
				}

			} else {
				filesMissingChecksums[relFile] = nil
			}

		}
	}

	return nil
}

func generateChecksumForFile(id, manualUrl, relFile string) error {
	gca := nod.NewProgress("generating checksums for %s...", relFile)

	vf, err := generateChecksumData(relFile)
	if err != nil {
		return gca.EndWithError(err)
	}

	absChecksum, err := vangogh_local_data.AbsLocalChecksumPath(relFile)
	if err != nil {
		return gca.EndWithError(err)
	}

	checksumFile, err := os.Create(absChecksum)
	if err != nil {
		return gca.EndWithError(err)
	}
	defer checksumFile.Close()

	if err := xml.NewEncoder(checksumFile).Encode(vf); err != nil {
		return gca.EndWithError(err)
	}

	gca.EndWithResult("done")

	return nil
}

func generateChecksumData(relFile string) (*vangogh_local_data.ValidationFile, error) {
	_, fname := filepath.Split(relFile)

	fa := nod.NewProgress(" %s", fname)
	defer fa.End()

	absFile, err := vangogh_local_data.AbsDownloadDirFromRel(relFile)
	if err != nil {
		return nil, fa.EndWithError(err)
	}

	inputFile, err := os.Open(absFile)
	if err != nil {
		return nil, fa.EndWithError(err)
	}

	h := md5.New()

	stat, err := inputFile.Stat()
	if err != nil {
		return nil, fa.EndWithError(err)
	}

	fa.Total(uint64(stat.Size()))

	if err := dolo.CopyWithProgress(h, inputFile, fa); err != nil {
		return nil, fa.EndWithError(err)
	}

	inputFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	vf := &vangogh_local_data.ValidationFile{
		XMLName:   xml.Name{},
		Name:      fname,
		Available: 1,
		MD5:       inputFileMD5,
		TotalSize: int(stat.Size()),
	}

	return vf, nil
}
