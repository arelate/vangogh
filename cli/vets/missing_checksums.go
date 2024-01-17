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
)

func MissingChecksums(fix bool) error {

	mca := nod.Begin("looking for missing checksums...")
	defer mca.End()

	rdx, err := vangogh_local_data.NewReduxReader(
		vangogh_local_data.ValidationResultProperty,
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.ProductTypeProperty)
	if err != nil {
		return mca.EndWithError(err)
	}

	ids := make(map[string]interface{})

	for _, id := range rdx.Keys(vangogh_local_data.ValidationResultProperty) {

		// skip DLC and PACK product types as they don't have Details for their ids and would
		// crash below attempting to read vrDetails for missing product. That's totally ok, since
		// DLC and PACK product types get validation status from cascading, so as we fix GAME
		// product types and perform cascade - we'll eventually get correct status for all cascaded types
		if pt, ok := rdx.GetFirstVal(vangogh_local_data.ProductTypeProperty, id); ok && pt != "GAME" {
			continue
		}

		if results, ok := rdx.GetAllValues(vangogh_local_data.ValidationResultProperty, id); ok {
			for _, res := range results {
				if res == "missing-checksum" {
					ids[id] = nil
				}
			}
		}
	}

	vrDetails, err := vangogh_local_data.NewReader(vangogh_local_data.Details)
	if err != nil {
		return mca.EndWithError(err)
	}

	filesMissingChecksums := make(map[string]interface{})

	for id := range ids {

		det, err := vrDetails.Details(id)
		if err != nil {
			return mca.EndWithError(err)
		}

		dls, err := vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			return mca.EndWithError(err)
		}

		for _, dl := range dls {
			relFile, ok := rdx.GetFirstVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl)
			if !ok {
				continue
			}

			if !vangogh_local_data.IsPathSupportingValidation(relFile) {
				continue
			}

			absChecksumFile, err := vangogh_local_data.AbsLocalChecksumPath(relFile)
			if err != nil {
				return mca.EndWithError(err)
			}
			if _, err := os.Stat(absChecksumFile); os.IsNotExist(err) {
				filesMissingChecksums[relFile] = nil
			}
		}
	}

	if fix {

		gca := nod.NewProgress("generating missing checksums (note: this assumes downloads have no issues)...")

		gca.TotalInt(len(filesMissingChecksums))

		for relFile := range filesMissingChecksums {
			vf, err := generateChecksumData(relFile)
			if err != nil {
				gca.Error(err)
			}

			absChecksum, err := vangogh_local_data.AbsLocalChecksumPath(relFile)
			if err != nil {
				return gca.EndWithError(err)
			}

			checksumFile, err := os.Create(absChecksum)
			if err != nil {
				return gca.EndWithError(err)
			}

			if err := xml.NewEncoder(checksumFile).Encode(vf); err != nil {
				return gca.EndWithError(err)
			}
		}

		gca.EndWithResult("done")

	} else {
		mca.EndWithResult("found %d local files missing checksums", len(filesMissingChecksums))
	}

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
	if err != nil {
		return nil, fa.EndWithError(err)
	}

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
