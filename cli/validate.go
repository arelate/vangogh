package cli

import (
	"crypto/md5"
	"encoding/xml"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/slices"
	"net/url"
	"os"
	"path/filepath"
)

func ValidateHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Validate(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.FlagFromUrl(u, "all-not-valid"))
}

func Validate(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	allNotValid bool) error {

	va := nod.NewProgress("validating...")
	defer va.End()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	rdx, err := vangogh_integration.NewReduxWriter(
		vangogh_integration.SlugProperty,
		vangogh_integration.NativeLanguageNameProperty,
		vangogh_integration.LocalManualUrlProperty,
		vangogh_integration.ManualUrlStatusProperty,
		vangogh_integration.ManualUrlValidationResultProperty,
		vangogh_integration.ManualUrlGeneratedChecksumProperty,
		vangogh_integration.ProductValidationResultProperty)
	if err != nil {
		return err
	}

	if allNotValid {
		ids, err = allNotValidIds(rdx)
		if err != nil {
			return va.EndWithError(err)
		}
	}

	vd := &validateDelegate{
		rdx:     rdx,
		results: make(map[vangogh_integration.ValidationResult]int),
	}

	if err := vangogh_integration.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		vd,
		va); err != nil {
		return err
	}

	summary := map[string][]string{}
	tp := fmt.Sprintf("%d manual-url(s) successfully validated", vd.results[vangogh_integration.ValidatedSuccessfully])
	summary[tp] = []string{}
	maybeAddTopic(summary, "%d manual-url(s) validated with generated checksum", vd.results, vangogh_integration.ValidatedWithGeneratedChecksum)
	maybeAddTopic(summary, "%d manual-url(s) are unresolved (not downloaded)", vd.results, vangogh_integration.ValidatedUnresolvedManualUrl)
	maybeAddTopic(summary, "%d manual-url(s) are missing downloads", vd.results, vangogh_integration.ValidatedMissingLocalFile)
	maybeAddTopic(summary, "%d manual-url(s) are missing checksum", vd.results, vangogh_integration.ValidatedMissingChecksum)
	maybeAddTopic(summary, "%d manual-url(s) have checksum mismatch", vd.results, vangogh_integration.ValidatedChecksumMismatch)
	maybeAddTopic(summary, "%d manual-url(s) have validation errors", vd.results, vangogh_integration.ValidationError)

	va.EndWithSummary("", summary)

	return CascadeValidation()
}

func allNotValidIds(rdx kevlar.ReadableRedux) ([]string, error) {

	avia := nod.NewProgress("itemizing all not valid products...")
	defer avia.EndWithResult("done")

	vrDetails, err := vangogh_integration.NewProductReader(vangogh_integration.Details)
	if err != nil {
		return nil, err
	}

	ids := make([]string, 0, vrDetails.Len())
	avia.TotalInt(vrDetails.Len())

	for id := range vrDetails.Keys() {

		if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
			if pvr := vangogh_integration.ParseValidationResult(pvrs); pvr == vangogh_integration.ValidatedSuccessfully || pvr == vangogh_integration.ValidatedWithGeneratedChecksum {
				avia.Increment()
				continue
			}
		}

		ids = append(ids, id)
		avia.Increment()
	}

	return ids, nil
}

func maybeAddTopic(summary map[string][]string,
	tmpl string,
	results map[vangogh_integration.ValidationResult]int,
	status vangogh_integration.ValidationResult) {
	if count, ok := results[status]; ok {
		tp := fmt.Sprintf(tmpl, count)
		summary[tp] = nil
	}
}

func validateManualUrl(
	dl *vangogh_integration.Download,
	rdx kevlar.ReadableRedux) (vangogh_integration.ValidationResult, error) {

	if err := rdx.MustHave(vangogh_integration.LocalManualUrlProperty); err != nil {
		return vangogh_integration.ValidationError, err
	}

	mua := nod.NewProgress(" %s:", dl.String())
	defer mua.EndWithResult("done")

	//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
	localFile, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl)
	if !ok {
		vr := vangogh_integration.ValidatedUnresolvedManualUrl
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	//absolute path (given a downloads/ root) for a s/slug/local_filename,
	//e.g. downloads/s/slug/local_filename
	absLocalFile, err := vangogh_integration.AbsDownloadDirFromRel(localFile)
	if err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}

	if _, err := os.Stat(absLocalFile); os.IsNotExist(err) {
		vr := vangogh_integration.ValidatedMissingLocalFile
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	absChecksumFile, err := vangogh_integration.AbsLocalChecksumPath(absLocalFile)
	if err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}

	if _, err := os.Stat(absChecksumFile); os.IsNotExist(err) {
		vr := vangogh_integration.ValidatedMissingChecksum
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	chkFile, err := os.Open(absChecksumFile)
	if err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}
	defer chkFile.Close()

	var chkData vangogh_integration.ValidationFile
	if err := xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}

	sourceFile, err := os.Open(absLocalFile)
	if err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}
	defer sourceFile.Close()

	h := md5.New()

	stat, err := sourceFile.Stat()
	if err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}

	_, filename := filepath.Split(localFile)
	vlfa := nod.NewProgress(" - %s", filename)

	vlfa.Total(uint64(stat.Size()))

	if err := dolo.CopyWithProgress(h, sourceFile, vlfa); err != nil {
		return vangogh_integration.ValidationError, mua.EndWithError(err)
	}

	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if chkData.MD5 != sourceFileMD5 {
		vr := vangogh_integration.ValidatedChecksumMismatch
		vlfa.EndWithResult(vr.String())
		return vr, nil
	} else {
		vr := vangogh_integration.ValidatedSuccessfully
		if gc, ok := rdx.GetLastVal(vangogh_integration.ManualUrlGeneratedChecksumProperty, dl.ManualUrl); ok && gc == vangogh_integration.TrueValue {
			vr = vangogh_integration.ValidatedWithGeneratedChecksum
		}
		vlfa.EndWithResult(vr.String())
		return vr, nil
	}
}

type validateDelegate struct {
	rdx     kevlar.WriteableRedux
	results map[vangogh_integration.ValidationResult]int
}

func downloadsListIsExtrasOnly(dls vangogh_integration.DownloadsList) bool {
	for _, dl := range dls {
		if dl.Type != vangogh_integration.Extra {
			return false
		}
	}
	return true
}

func (vd *validateDelegate) Process(id, slug string, list vangogh_integration.DownloadsList) error {

	sva := nod.Begin(slug)
	defer sva.End()

	manualUrlsValidationResults := make(map[string][]string)

	productVrs := make([]vangogh_integration.ValidationResult, 0, len(list))

	for _, dl := range list {

		vr, err := validateManualUrl(&dl, vd.rdx)
		if err != nil {
			vr = vangogh_integration.ValidationError
			sva.Error(err)
		}

		if dl.Type == vangogh_integration.Installer || dl.Type == vangogh_integration.DLC {
			productVrs = append(productVrs, vr)
		}

		manualUrlsValidationResults[dl.ManualUrl] = []string{vr.String()}
		vd.results[vr] = vd.results[vr] + 1

		if err := vd.rdx.BatchReplaceValues(vangogh_integration.ManualUrlValidationResultProperty, manualUrlsValidationResults); err != nil {
			return sva.EndWithError(err)
		}

		if err := vd.rdx.ReplaceValues(
			vangogh_integration.ManualUrlStatusProperty,
			dl.ManualUrl,
			vangogh_integration.ManualUrlValidated.String()); err != nil {
			return sva.EndWithError(err)
		}
	}

	slices.Sort(productVrs)

	productValidationResult := vangogh_integration.ValidationResultUnknown

	if downloadsListIsExtrasOnly(list) {
		productValidationResult = vangogh_integration.ValidatedSuccessfully
	} else if len(productVrs) > 0 {
		productValidationResult = productVrs[len(productVrs)-1]
	}

	if err := vd.rdx.ReplaceValues(vangogh_integration.ProductValidationResultProperty, id, productValidationResult.String()); err != nil {
		return sva.EndWithError(err)
	}

	sva.EndWithResult(productValidationResult.String())

	return nil
}

func validateUpdated(ids []string,
	since int64,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool) error {

	if ids == nil {
		var err error
		ids, err = itemizeUpdatedAccountProducts(since)
		if err != nil {
			return err
		}
	}

	return Validate(ids, operatingSystems, langCodes, downloadTypes, noPatches, false)
}
