package cli

import (
	"crypto/md5"
	"encoding/xml"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/url"
	"os"
	"path/filepath"
	"slices"
)

func ValidateHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Validate(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "all-not-valid"))
}

func Validate(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	allNotValid bool) error {

	va := nod.NewProgress("validating...")
	defer va.Done()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.SlugProperty,
		vangogh_integration.ManualUrlFilenameProperty,
		vangogh_integration.ManualUrlStatusProperty,
		vangogh_integration.ManualUrlValidationResultProperty,
		vangogh_integration.ProductValidationResultProperty)
	if err != nil {
		return err
	}

	if allNotValid {
		ids, err = allNotValidIds(rdx)
		if err != nil {
			return err
		}
	}

	vd := &validateDelegate{
		rdx:             rdx,
		downloadsLayout: downloadsLayout,
		results:         make(map[vangogh_integration.ValidationResult]int),
	}

	if err = vangogh_integration.MapDownloads(
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
	maybeAddTopic(summary, "%d manual-url(s) are unresolved (not downloaded)", vd.results, vangogh_integration.ValidatedUnresolvedManualUrl)
	maybeAddTopic(summary, "%d manual-url(s) are missing downloads", vd.results, vangogh_integration.ValidatedMissingLocalFile)
	maybeAddTopic(summary, "%d manual-url(s) are missing checksum", vd.results, vangogh_integration.ValidatedMissingChecksum)
	maybeAddTopic(summary, "%d manual-url(s) have checksum mismatch", vd.results, vangogh_integration.ValidatedChecksumMismatch)
	maybeAddTopic(summary, "%d manual-url(s) have validation errors", vd.results, vangogh_integration.ValidationError)

	va.EndWithSummary("", summary)

	return CascadeValidation()
}

func allNotValidIds(rdx redux.Readable) ([]string, error) {

	avia := nod.NewProgress("itemizing all not valid products...")
	defer avia.Done()

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return nil, err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	ids := make([]string, 0, kvDetails.Len())
	avia.TotalInt(kvDetails.Len())

	for id := range kvDetails.Keys() {

		if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
			if pvr := vangogh_integration.ParseValidationResult(pvrs); pvr == vangogh_integration.ValidatedSuccessfully {
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
	slug string,
	dl *vangogh_integration.Download,
	rdx redux.Readable,
	layout vangogh_integration.DownloadsLayout) (vangogh_integration.ValidationResult, error) {

	mua := nod.NewProgress(" %s:", dl.String())
	defer mua.Done()

	filename, ok := rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl)
	if !ok || filename == "" {
		vr := vangogh_integration.ValidatedUnresolvedManualUrl
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.Type, layout)
	if err != nil {
		return vangogh_integration.ValidationError, err
	}

	absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

	if _, err := os.Stat(absDownloadPath); os.IsNotExist(err) {
		vr := vangogh_integration.ValidatedMissingLocalFile
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	absChecksumFile, err := vangogh_integration.AbsChecksumPath(absDownloadPath)
	if err != nil {
		return vangogh_integration.ValidationError, err
	}

	if _, err := os.Stat(absChecksumFile); os.IsNotExist(err) {
		vr := vangogh_integration.ValidatedMissingChecksum
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	chkFile, err := os.Open(absChecksumFile)
	if err != nil {
		return vangogh_integration.ValidationError, err
	}
	defer chkFile.Close()

	var chkData vangogh_integration.ValidationFile
	if err := xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return vangogh_integration.ValidationError, err
	}

	sourceFile, err := os.Open(absDownloadPath)
	if err != nil {
		return vangogh_integration.ValidationError, err
	}
	defer sourceFile.Close()

	h := md5.New()

	stat, err := sourceFile.Stat()
	if err != nil {
		return vangogh_integration.ValidationError, err
	}

	vlfa := nod.NewProgress(" - %s", filename)

	vlfa.Total(uint64(stat.Size()))

	if err := dolo.CopyWithProgress(h, sourceFile, vlfa); err != nil {
		return vangogh_integration.ValidationError, err
	}

	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if chkData.MD5 != sourceFileMD5 {
		vr := vangogh_integration.ValidatedChecksumMismatch
		vlfa.EndWithResult(vr.String())
		return vr, nil
	} else {
		vr := vangogh_integration.ValidatedSuccessfully
		vlfa.EndWithResult(vr.String())
		return vr, nil
	}
}

type validateDelegate struct {
	rdx             redux.Writeable
	downloadsLayout vangogh_integration.DownloadsLayout
	results         map[vangogh_integration.ValidationResult]int
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
	defer sva.Done()

	manualUrlsValidationResults := make(map[string][]string)

	productVrs := make([]vangogh_integration.ValidationResult, 0, len(list))

	for _, dl := range list {

		if dl.Type != vangogh_integration.Installer && dl.Type != vangogh_integration.DLC {
			continue
		}

		vr, err := validateManualUrl(slug, &dl, vd.rdx, vd.downloadsLayout)
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
			return err
		}

		if err := vd.rdx.ReplaceValues(
			vangogh_integration.ManualUrlStatusProperty,
			dl.ManualUrl,
			vangogh_integration.ManualUrlValidated.String()); err != nil {
			return err
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
		return err
	}

	sva.EndWithResult(productValidationResult.String())

	return nil
}
