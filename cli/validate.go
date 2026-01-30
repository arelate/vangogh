package cli

import (
	"crypto/md5"
	"encoding/xml"
	"fmt"
	"net/url"
	"os"
	"path/filepath"
	"slices"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

type validationOptions struct {
	validationStatuses []vangogh_integration.ValidationStatus
	notValid           bool
	all                bool
}

func ValidateHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	q := u.Query()

	var validationStatuses []vangogh_integration.ValidationStatus
	if q.Has("validation-status") {
		for _, vs := range strings.Split(q.Get("validation-status"), ",") {
			validationStatuses = append(validationStatuses, vangogh_integration.ParseValidationStatus(vs))
		}
	}

	vo := &validationOptions{
		validationStatuses: validationStatuses,
		notValid:           q.Has("not-valid"),
		all:                q.Has("all"),
	}

	return Validate(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		q.Has("no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		vo)
}

func Validate(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	vo *validationOptions) error {

	va := nod.NewProgress("validating...")
	defer va.Done()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.SlugProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.ManualUrlFilenameProperty,
		vangogh_integration.ManualUrlStatusProperty,
		vangogh_integration.ManualUrlValidationResultProperty,
		vangogh_integration.ProductValidationResultProperty,
		vangogh_integration.ProductValidationDateProperty)
	if err != nil {
		return err
	}

	if len(vo.validationStatuses) > 0 {
		ids, err = validationStatusIds(rdx, vo.validationStatuses...)
		if err != nil {
			return err
		}
	}

	if vo.notValid {

		var notValidStatuses []vangogh_integration.ValidationStatus
		for _, vs := range vangogh_integration.AllValidationStatuses() {
			if vs == vangogh_integration.ValidationStatusSuccess {
				continue
			}
			notValidStatuses = append(notValidStatuses, vs)
		}

		ids, err = validationStatusIds(rdx, notValidStatuses...)
		if err != nil {
			return err
		}
	}

	if vo.all {
		detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
		if err != nil {
			return err
		}

		kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
		if err != nil {
			return err
		}

		ids = slices.Collect(kvDetails.Keys())
	}

	vd := &validateDelegate{
		rdx:             rdx,
		downloadsLayout: downloadsLayout,
		statuses:        make(map[vangogh_integration.ValidationStatus]int),
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
	tp := fmt.Sprintf("%d manual-url(s) successfully validated", vd.statuses[vangogh_integration.ValidationStatusSuccess])
	summary[tp] = []string{}
	maybeAddTopic(summary, "%d manual-url(s) are unresolved (not downloaded)", vd.statuses, vangogh_integration.ValidationStatusUnresolvedManualUrl)
	maybeAddTopic(summary, "%d manual-url(s) are missing downloads", vd.statuses, vangogh_integration.ValidationStatusMissingLocalFile)
	maybeAddTopic(summary, "%d manual-url(s) are missing checksum", vd.statuses, vangogh_integration.ValidationStatusMissingChecksum)
	maybeAddTopic(summary, "%d manual-url(s) have checksum mismatch", vd.statuses, vangogh_integration.ValidationStatusChecksumMismatch)
	maybeAddTopic(summary, "%d manual-url(s) have validation errors", vd.statuses, vangogh_integration.ValidationStatusError)

	va.EndWithSummary("", summary)

	return nil
}

func validationStatusIds(rdx redux.Readable, validationStatuses ...vangogh_integration.ValidationStatus) ([]string, error) {

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

		if pvss, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
			if pvs := vangogh_integration.ParseValidationStatus(pvss); slices.Contains(validationStatuses, pvs) {
				ids = append(ids, id)
			}
		}

		avia.Increment()
	}

	return ids, nil
}

func maybeAddTopic(summary map[string][]string,
	tmpl string,
	statuses map[vangogh_integration.ValidationStatus]int,
	status vangogh_integration.ValidationStatus) {
	if count, ok := statuses[status]; ok {
		tp := fmt.Sprintf(tmpl, count)
		summary[tp] = nil
	}
}

func validateManualUrl(
	slug string,
	dl *vangogh_integration.Download,
	rdx redux.Readable,
	layout vangogh_integration.DownloadsLayout) (vangogh_integration.ValidationStatus, error) {

	mua := nod.NewProgress(" %s:", dl.String())
	defer mua.Done()

	filename, ok := rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl)
	if !ok || filename == "" {
		vs := vangogh_integration.ValidationStatusUnresolvedManualUrl
		mua.EndWithResult(vs.String())
		return vs, nil
	}

	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.DownloadType, layout)
	if err != nil {
		return vangogh_integration.ValidationStatusError, err
	}

	absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

	if _, err = os.Stat(absDownloadPath); os.IsNotExist(err) {
		vr := vangogh_integration.ValidationStatusMissingLocalFile
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	absChecksumFile, err := vangogh_integration.AbsChecksumPath(absDownloadPath)
	if err != nil {
		return vangogh_integration.ValidationStatusError, err
	}

	if _, err = os.Stat(absChecksumFile); os.IsNotExist(err) {
		vr := vangogh_integration.ValidationStatusMissingChecksum
		mua.EndWithResult(vr.String())
		return vr, nil
	}

	chkFile, err := os.Open(absChecksumFile)
	if err != nil {
		return vangogh_integration.ValidationStatusError, err
	}
	defer chkFile.Close()

	var chkData vangogh_integration.ValidationFile
	if err = xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return vangogh_integration.ValidationStatusError, err
	}

	sourceFile, err := os.Open(absDownloadPath)
	if err != nil {
		return vangogh_integration.ValidationStatusError, err
	}
	defer sourceFile.Close()

	h := md5.New()

	stat, err := sourceFile.Stat()
	if err != nil {
		return vangogh_integration.ValidationStatusError, err
	}

	vlfa := nod.NewProgress(" - %s", filename)

	vlfa.Total(uint64(stat.Size()))

	if err = dolo.CopyWithProgress(h, sourceFile, vlfa); err != nil {
		return vangogh_integration.ValidationStatusError, err
	}

	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if chkData.MD5 != sourceFileMD5 {
		vr := vangogh_integration.ValidationStatusChecksumMismatch
		vlfa.EndWithResult(vr.String())
		return vr, nil
	} else {
		vr := vangogh_integration.ValidationStatusSuccess
		vlfa.EndWithResult(vr.String())
		return vr, nil
	}
}

type validateDelegate struct {
	rdx             redux.Writeable
	downloadsLayout vangogh_integration.DownloadsLayout
	statuses        map[vangogh_integration.ValidationStatus]int
}

func downloadsListIsExtrasOnly(dls vangogh_integration.DownloadsList) bool {
	for _, dl := range dls {
		if dl.DownloadType != vangogh_integration.Extra {
			return false
		}
	}
	return true
}

func (vd *validateDelegate) Process(id, slug string, list vangogh_integration.DownloadsList) error {

	sva := nod.Begin("validating %s...", slug)
	defer sva.Done()

	if err := vd.rdx.ReplaceValues(vangogh_integration.ProductValidationResultProperty,
		id,
		vangogh_integration.ValidationStatusValidating.String()); err != nil {
		return err
	}

	if err := vd.rdx.CutKeys(vangogh_integration.ProductValidationDateProperty, id); err != nil {
		return err
	}

	manualUrlsValidationResults := make(map[string][]string)

	productVrs := make([]vangogh_integration.ValidationStatus, 0, len(list))

	validationQueued := make(map[string][]string)
	for _, dl := range list {
		validationQueued[dl.ManualUrl] = []string{vangogh_integration.ValidationStatusQueued.String()}
	}

	if err := vd.rdx.BatchReplaceValues(vangogh_integration.ManualUrlValidationResultProperty, validationQueued); err != nil {
		return err
	}

	for _, dl := range list {

		if dl.DownloadType != vangogh_integration.Installer && dl.DownloadType != vangogh_integration.DLC {
			continue
		}

		if err := vd.rdx.ReplaceValues(
			vangogh_integration.ManualUrlValidationResultProperty,
			dl.ManualUrl,
			vangogh_integration.ValidationStatusValidating.String()); err != nil {
			return err
		}

		vr, err := validateManualUrl(slug, &dl, vd.rdx, vd.downloadsLayout)
		if err != nil {
			vr = vangogh_integration.ValidationStatusError
			sva.Error(err)
		}

		if dl.DownloadType == vangogh_integration.Installer || dl.DownloadType == vangogh_integration.DLC {
			productVrs = append(productVrs, vr)
		}

		manualUrlsValidationResults[dl.ManualUrl] = []string{vr.String()}
		vd.statuses[vr] = vd.statuses[vr] + 1

		if err = vd.rdx.BatchReplaceValues(vangogh_integration.ManualUrlValidationResultProperty, manualUrlsValidationResults); err != nil {
			return err
		}

		if err = vd.rdx.ReplaceValues(
			vangogh_integration.ManualUrlStatusProperty,
			dl.ManualUrl,
			vangogh_integration.DownloadStatusValidated.String()); err != nil {
			return err
		}
	}

	productValidationResult := vangogh_integration.ValidationStatusUnknown

	if downloadsListIsExtrasOnly(list) {
		productValidationResult = vangogh_integration.ValidationStatusSuccess
	} else if len(productVrs) > 0 {
		productValidationResult = vangogh_integration.WorstValidationStatus(productVrs...)
	}

	if err := vd.rdx.ReplaceValues(vangogh_integration.ProductValidationResultProperty, id, productValidationResult.String()); err != nil {
		return err
	}

	if err := vd.rdx.ReplaceValues(vangogh_integration.ProductValidationDateProperty, id, time.Now().UTC().Format(nod.TimeFormat)); err != nil {
		return err
	}

	sva.EndWithResult(productValidationResult.String())

	return nil
}
