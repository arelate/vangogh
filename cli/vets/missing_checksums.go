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

func MissingChecksums(
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	fix bool) error {

	mca := nod.NewProgress("checking for missing checksums...")
	defer mca.End()

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.ManualUrlStatusProperty,
		vangogh_local_data.ManualUrlValidationResultProperty,
		vangogh_local_data.ManualUrlGeneratedChecksumProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.ProductTypeProperty)
	if err != nil {
		return mca.EndWithError(err)
	}

	manualUrlsMissingChecksums := make([]string, 0)

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return mca.EndWithError(err)
	}

	keys, err := vrDetails.Keys()
	if err != nil {
		return mca.EndWithError(err)
	}

	mca.TotalInt(len(keys))

	for _, id := range keys {

		// skip DLC and PACK product types as they don't have Details for their ids and would
		// crash below attempting to read vrDetails for missing product. That's totally ok, since
		// DLC and PACK product types get validation status from cascading, so as we fix GAME
		// product types and perform cascade - we'll eventually get correct status for all cascaded types
		if pt, ok := rdx.GetLastVal(vangogh_local_data.ProductTypeProperty, id); ok && pt != "GAME" {
			continue
		}

		det, err := vrDetails.Details(id)
		if err != nil {
			return mca.EndWithError(err)
		}

		dls, err := vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			return mca.EndWithError(err)
		}

		dls = dls.Only(operatingSystems, langCodes, downloadTypes, noPatches)

		for _, dl := range dls {
			if muss, ok := rdx.GetLastVal(vangogh_local_data.ManualUrlStatusProperty, dl.ManualUrl); ok {
				if vangogh_local_data.ParseManualUrlStatus(muss) != vangogh_local_data.ManualUrlValidated {
					continue
				}
			}

			if vrs, ok := rdx.GetLastVal(vangogh_local_data.ManualUrlValidationResultProperty, dl.ManualUrl); ok {
				if vangogh_local_data.ParseValidationResult(vrs) == vangogh_local_data.ValidatedMissingChecksum {
					manualUrlsMissingChecksums = append(manualUrlsMissingChecksums, dl.ManualUrl)
				}
			}
		}
		mca.Increment()

	}

	filesMissingChecksums := make(map[string]interface{})

	for _, manualUrl := range manualUrlsMissingChecksums {

		relFile, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, manualUrl)
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

			if err := generateChecksumForFile(relFile); err != nil {
				return mca.EndWithError(err)
			}

			if err := rdx.AddValues(vangogh_local_data.ManualUrlGeneratedChecksumProperty, manualUrl, vangogh_local_data.TrueValue); err != nil {
				return mca.EndWithError(err)
			}

		} else {
			filesMissingChecksums[relFile] = nil
		}

	}

	return nil
}

func generateChecksumForFile(relFile string) error {
	gca := nod.NewProgress("generating checksums for %s...", relFile)

	vf, err := generateChecksumData(relFile)
	if err != nil {
		return gca.EndWithError(err)
	}

	absChecksumPath, err := vangogh_local_data.AbsLocalChecksumPath(relFile)
	if err != nil {
		return gca.EndWithError(err)
	}

	absChecksumDir, _ := filepath.Split(absChecksumPath)
	if _, err := os.Stat(absChecksumDir); os.IsNotExist(err) {
		if err := os.MkdirAll(absChecksumDir, 0755); err != nil {
			return gca.EndWithError(err)
		}
	}

	checksumFile, err := os.Create(absChecksumPath)
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
