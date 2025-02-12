package vets

import (
	"crypto/md5"
	"encoding/xml"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"os"
	"path/filepath"
)

func MissingChecksums(
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noPatches bool,
	fix bool) error {

	mca := nod.NewProgress("checking for missing checksums...")
	defer mca.Done()

	rdx, err := vangogh_integration.NewReduxWriter(
		vangogh_integration.LocalManualUrlProperty,
		vangogh_integration.ManualUrlStatusProperty,
		vangogh_integration.ManualUrlValidationResultProperty,
		vangogh_integration.ManualUrlGeneratedChecksumProperty,
		//vangogh_integration.NativeLanguageNameProperty,
		vangogh_integration.ProductTypeProperty)
	if err != nil {
		return err
	}

	manualUrlsMissingChecksums := make([]string, 0)

	vrDetails, err := vangogh_integration.NewProductReader(vangogh_integration.Details)
	if err != nil {
		return err
	}

	mca.TotalInt(vrDetails.Len())

	for id := range vrDetails.Keys() {

		// skip DLC and PACK product types as they don't have Details for their ids and would
		// crash below attempting to read vrDetails for missing product. That's totally ok, since
		// DLC and PACK product types get validation status from cascading, so as we fix GAME
		// product types and perform cascade - we'll eventually get correct status for all cascaded types
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok && pt != "GAME" {
			continue
		}

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		dls, err := vangogh_integration.FromDetails(det, rdx)
		if err != nil {
			return err
		}

		dls = dls.Only(operatingSystems,
			langCodes,
			[]vangogh_integration.DownloadType{vangogh_integration.Installer, vangogh_integration.DLC},
			noPatches)

		for _, dl := range dls {
			if muss, ok := rdx.GetLastVal(vangogh_integration.ManualUrlStatusProperty, dl.ManualUrl); ok {
				if vangogh_integration.ParseManualUrlStatus(muss) != vangogh_integration.ManualUrlValidated {
					continue
				}
			}

			if vrs, ok := rdx.GetLastVal(vangogh_integration.ManualUrlValidationResultProperty, dl.ManualUrl); ok {
				if vangogh_integration.ParseValidationResult(vrs) == vangogh_integration.ValidatedMissingChecksum {
					manualUrlsMissingChecksums = append(manualUrlsMissingChecksums, dl.ManualUrl)
				}
			}
		}
		mca.Increment()

	}

	filesMissingChecksums := make(map[string]interface{})

	for _, manualUrl := range manualUrlsMissingChecksums {

		relFile, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, manualUrl)
		if !ok {
			continue
		}

		absChecksumFile, err := vangogh_integration.AbsLocalChecksumPath(relFile)
		if err != nil {
			return err
		}
		if _, err := os.Stat(absChecksumFile); err == nil {
			continue
		}

		if fix {

			if err := generateChecksumForFile(relFile); err != nil {
				return err
			}

			if err := rdx.AddValues(vangogh_integration.ManualUrlGeneratedChecksumProperty, manualUrl, vangogh_integration.TrueValue); err != nil {
				return err
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
		return err
	}

	absChecksumPath, err := vangogh_integration.AbsLocalChecksumPath(relFile)
	if err != nil {
		return err
	}

	absChecksumDir, _ := filepath.Split(absChecksumPath)
	if _, err := os.Stat(absChecksumDir); os.IsNotExist(err) {
		if err := os.MkdirAll(absChecksumDir, 0755); err != nil {
			return err
		}
	}

	checksumFile, err := os.Create(absChecksumPath)
	if err != nil {
		return err
	}
	defer checksumFile.Close()

	if err := xml.NewEncoder(checksumFile).Encode(vf); err != nil {
		return err
	}

	gca.Done()

	return nil
}

func generateChecksumData(relFile string) (*vangogh_integration.ValidationFile, error) {
	_, fname := filepath.Split(relFile)

	fa := nod.NewProgress(" %s", fname)
	defer fa.Done()

	absFile, err := vangogh_integration.AbsDownloadDirFromRel(relFile)
	if err != nil {
		return nil, err
	}

	inputFile, err := os.Open(absFile)
	if err != nil {
		return nil, err
	}

	h := md5.New()

	stat, err := inputFile.Stat()
	if err != nil {
		return nil, err
	}

	fa.Total(uint64(stat.Size()))

	if err := dolo.CopyWithProgress(h, inputFile, fa); err != nil {
		return nil, err
	}

	inputFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	vf := &vangogh_integration.ValidationFile{
		XMLName:   xml.Name{},
		Name:      fname,
		Available: 1,
		MD5:       inputFileMD5,
		TotalSize: int(stat.Size()),
	}

	return vf, nil
}
