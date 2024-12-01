package cli

import (
	"crypto/md5"
	"encoding/xml"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/slices"
	"net/url"
	"os"
	"path/filepath"
)

func ValidateHandler(u *url.URL) error {
	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Validate(
		ids,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		vangogh_local_data.FlagFromUrl(u, "all-not-valid"))
}

func Validate(
	ids []string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	allNotValid bool) error {

	va := nod.NewProgress("validating...")
	defer va.End()

	vangogh_local_data.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.SlugProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.ManualUrlStatusProperty,
		vangogh_local_data.ManualUrlValidationResultProperty,
		vangogh_local_data.ManualUrlGeneratedChecksumProperty,
		vangogh_local_data.ProductValidationResultProperty)
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
		results: make(map[vangogh_local_data.ValidationResult]int),
	}

	if err := vangogh_local_data.MapDownloads(
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
	tp := fmt.Sprintf("%d manual-url(s) successfully validated", vd.results[vangogh_local_data.ValidatedSuccessfully])
	summary[tp] = []string{}
	maybeAddTopic(summary, "%d manual-url(s) validated with generated checksum", vd.results, vangogh_local_data.ValidatedWithGeneratedChecksum)
	maybeAddTopic(summary, "%d manual-url(s) are unresolved (not downloaded)", vd.results, vangogh_local_data.ValidatedUnresolvedManualUrl)
	maybeAddTopic(summary, "%d manual-url(s) are missing downloads", vd.results, vangogh_local_data.ValidatedMissingLocalFile)
	maybeAddTopic(summary, "%d manual-url(s) are missing checksum", vd.results, vangogh_local_data.ValidatedMissingChecksum)
	maybeAddTopic(summary, "%d manual-url(s) have checksum mismatch", vd.results, vangogh_local_data.ValidatedChecksumMismatch)
	maybeAddTopic(summary, "%d manual-url(s) have validation errors", vd.results, vangogh_local_data.ValidationError)

	va.EndWithSummary("", summary)

	return nil
}

func allNotValidIds(rdx kevlar.ReadableRedux) ([]string, error) {

	avia := nod.NewProgress("itemizing all not valid products...")
	defer avia.EndWithResult("done")

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return nil, err
	}
	keys, err := vrDetails.Keys()
	if err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(keys))

	avia.TotalInt(len(keys))

	for _, id := range keys {

		if pvrs, ok := rdx.GetLastVal(vangogh_local_data.ProductValidationResultProperty, id); ok {
			if pvr := vangogh_local_data.ParseValidationResult(pvrs); pvr == vangogh_local_data.ValidatedSuccessfully || pvr == vangogh_local_data.ValidatedWithGeneratedChecksum {
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
	results map[vangogh_local_data.ValidationResult]int,
	status vangogh_local_data.ValidationResult) {
	if count, ok := results[status]; ok {
		tp := fmt.Sprintf(tmpl, count)
		summary[tp] = nil
	}
}

func validateManualUrl(
	dl *vangogh_local_data.Download,
	rdx kevlar.ReadableRedux) (vangogh_local_data.ValidationResult, error) {

	if err := rdx.MustHave(vangogh_local_data.LocalManualUrlProperty); err != nil {
		return vangogh_local_data.ValidationError, err
	}

	mua := nod.NewProgress(" %s:", dl.String())
	defer mua.EndWithResult("done")

	//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
	localFile, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl)
	if !ok {
		vr := vangogh_local_data.ValidatedUnresolvedManualUrl
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	//absolute path (given a downloads/ root) for a s/slug/local_filename,
	//e.g. downloads/s/slug/local_filename
	absLocalFile, err := vangogh_local_data.AbsDownloadDirFromRel(localFile)
	if err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}

	if _, err := os.Stat(absLocalFile); os.IsNotExist(err) {
		vr := vangogh_local_data.ValidatedMissingLocalFile
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	absChecksumFile, err := vangogh_local_data.AbsLocalChecksumPath(absLocalFile)
	if err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}

	if _, err := os.Stat(absChecksumFile); os.IsNotExist(err) {
		vr := vangogh_local_data.ValidatedMissingChecksum
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	chkFile, err := os.Open(absChecksumFile)
	if err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}
	defer chkFile.Close()

	var chkData vangogh_local_data.ValidationFile
	if err := xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}

	sourceFile, err := os.Open(absLocalFile)
	if err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}
	defer sourceFile.Close()

	h := md5.New()

	stat, err := sourceFile.Stat()
	if err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}

	_, filename := filepath.Split(localFile)
	vlfa := nod.NewProgress(" - %s", filename)

	vlfa.Total(uint64(stat.Size()))

	if err := dolo.CopyWithProgress(h, sourceFile, vlfa); err != nil {
		return vangogh_local_data.ValidationError, mua.EndWithError(err)
	}

	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if chkData.MD5 != sourceFileMD5 {
		vr := vangogh_local_data.ValidatedChecksumMismatch
		vlfa.EndWithResult(vr.String())
		return vr, nil
	} else {
		vr := vangogh_local_data.ValidatedSuccessfully
		if gc, ok := rdx.GetLastVal(vangogh_local_data.ManualUrlGeneratedChecksumProperty, dl.ManualUrl); ok && gc == vangogh_local_data.TrueValue {
			vr = vangogh_local_data.ValidatedWithGeneratedChecksum
		}
		vlfa.EndWithResult(vr.String())
		return vr, nil
	}
}

type validateDelegate struct {
	rdx     kevlar.WriteableRedux
	results map[vangogh_local_data.ValidationResult]int
}

func downloadsListIsExtrasOnly(dls vangogh_local_data.DownloadsList) bool {
	for _, dl := range dls {
		if dl.Type != vangogh_local_data.Extra {
			return false
		}
	}
	return true
}

func (vd *validateDelegate) Process(id, slug string, list vangogh_local_data.DownloadsList) error {

	sva := nod.Begin(slug)
	defer sva.End()

	manualUrlsValidationResults := make(map[string][]string)

	productVrs := make([]vangogh_local_data.ValidationResult, 0, len(list))

	for _, dl := range list {

		vr, err := validateManualUrl(&dl, vd.rdx)
		if err != nil {
			vr = vangogh_local_data.ValidationError
			sva.Error(err)
		}

		if dl.Type == vangogh_local_data.Installer || dl.Type == vangogh_local_data.DLC {
			productVrs = append(productVrs, vr)
		}

		manualUrlsValidationResults[dl.ManualUrl] = []string{vr.String()}
		vd.results[vr] = vd.results[vr] + 1

		if err := vd.rdx.BatchReplaceValues(vangogh_local_data.ManualUrlValidationResultProperty, manualUrlsValidationResults); err != nil {
			return sva.EndWithError(err)
		}

		if err := vd.rdx.ReplaceValues(
			vangogh_local_data.ManualUrlStatusProperty,
			dl.ManualUrl,
			vangogh_local_data.ManualUrlValidated.String()); err != nil {
			return sva.EndWithError(err)
		}
	}

	slices.Sort(productVrs)

	productValidationResult := vangogh_local_data.ValidationResultUnknown

	if downloadsListIsExtrasOnly(list) {
		productValidationResult = vangogh_local_data.ValidatedSuccessfully
	} else if len(productVrs) > 0 {
		productValidationResult = productVrs[len(productVrs)-1]
	}

	if err := vd.rdx.ReplaceValues(vangogh_local_data.ProductValidationResultProperty, id, productValidationResult.String()); err != nil {
		return sva.EndWithError(err)
	}

	sva.EndWithResult(productValidationResult.String())

	return nil
}

func validateUpdated(ids []string,
	since int64,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
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
