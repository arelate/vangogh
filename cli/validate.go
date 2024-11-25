package cli

import (
	"crypto/md5"
	"encoding/xml"
	"errors"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/url"
	"os"
	"path/filepath"
)

var (
	ErrUnresolvedManualUrl    = errors.New("unresolved manual-url")
	ErrMissingDownload        = errors.New("not downloaded")
	ErrMissingChecksum        = errors.New("missing checksum")
	ErrValidationNotSupported = errors.New("validation not supported")
	ErrValidationFailed       = errors.New("failed validation")
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
		vangogh_local_data.FlagFromUrl(u, "all"),
		vangogh_local_data.FlagFromUrl(u, "skip-valid"))
}

func Validate(
	ids []string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	excludePatches bool,
	all bool,
	skipValid bool) error {

	va := nod.NewProgress("validating...")
	defer va.End()

	vangogh_local_data.PrintParams(ids, operatingSystems, langCodes, downloadTypes)

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.SlugProperty,
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.ManualUrlStatusProperty)
	//vangogh_local_data.ValidationResultProperty,
	//vangogh_local_data.ValidationCompletedProperty)
	if err != nil {
		return err
	}

	if all {
		//vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
		//if err != nil {
		//	return err
		//}
		//keys, err := vrDetails.Keys()
		//if err != nil {
		//	return err
		//}
		//for _, id := range keys {
		//	if skipValid {
		//		valid, ok := rdx.GetLastVal(vangogh_local_data.ValidationResultProperty, id)
		//		if ok && valid == vangogh_local_data.OKValue {
		//			continue
		//		}
		//	}
		//	ids = append(ids, id)
		//}
	}

	vd := &validateDelegate{rdx: rdx}

	if err := vangogh_local_data.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		downloadTypes,
		langCodes,
		excludePatches,
		vd,
		va); err != nil {
		return err
	}

	summary := map[string][]string{}
	tp := fmt.Sprintf("%d product(s) successfully validated", len(vd.validated))
	summary[tp] = []string{}
	maybeAddTopic(summary, "%d product(s) have unresolved manual-url (not downloaded)", vd.unresolvedManualUrl)
	maybeAddTopic(summary, "%d product(s) missing downloads", vd.missingDownloads)
	maybeAddTopic(summary, "%d product(s) without checksum", vd.missingChecksum)
	maybeAddTopic(summary, "%d product extra(s) without checksum", vd.missingExtraChecksum)
	maybeAddTopic(summary, "%d product(s) failed validation", vd.failed)
	if len(vd.slugLastError) > 0 {
		tp = fmt.Sprintf("%d product(s) validation caused an error", len(vd.slugLastError))
		summary[tp] = make([]string, 0, len(vd.slugLastError))
		for slug, err := range vd.slugLastError {
			summary[tp] = append(summary[tp], fmt.Sprintf(" %s: %s", slug, err))
		}
	}

	va.EndWithSummary("", summary)

	return nil
}

func maybeAddTopic(summary map[string][]string, tmpl string, col map[string]bool) {
	if len(col) > 0 {
		tp := fmt.Sprintf(tmpl, len(col))
		summary[tp] = make([]string, 0, len(col))
		for it := range col {
			summary[tp] = append(summary[tp], it)
		}
	}
}

func validateManualUrl(
	dl *vangogh_local_data.Download,
	rdx kevlar.ReadableRedux) error {

	if err := rdx.MustHave(vangogh_local_data.LocalManualUrlProperty); err != nil {
		return err
	}

	mua := nod.NewProgress(" %s:", dl.String())
	defer mua.End()

	//local filenames are saved as relative to root downloads folder (e.g. s/slug/local_filename)
	localFile, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl)
	if !ok {
		mua.EndWithResult(ErrUnresolvedManualUrl.Error())
		return ErrUnresolvedManualUrl
	}

	//absolute path (given a downloads/ root) for a s/slug/local_filename,
	//e.g. downloads/s/slug/local_filename
	absLocalFile, err := vangogh_local_data.AbsDownloadDirFromRel(localFile)
	if err != nil {
		return mua.EndWithError(err)
	}
	if !vangogh_local_data.IsPathSupportingValidation(absLocalFile) {
		if _, err := os.Stat(absLocalFile); err == nil {
			mua.EndWithResult(ErrValidationNotSupported.Error())
			return ErrValidationNotSupported
		} else {
			mua.EndWithResult(ErrMissingDownload.Error())
			return ErrMissingDownload
		}
	}

	if _, err := os.Stat(absLocalFile); os.IsNotExist(err) {
		mua.EndWithResult(ErrMissingDownload.Error())
		return ErrMissingDownload
	}

	absChecksumFile, err := vangogh_local_data.AbsLocalChecksumPath(absLocalFile)
	if err != nil {
		return mua.EndWithError(err)
	}

	if _, err := os.Stat(absChecksumFile); os.IsNotExist(err) {
		mua.EndWithResult(ErrMissingChecksum.Error())
		return ErrMissingChecksum
	}

	chkFile, err := os.Open(absChecksumFile)
	if err != nil {
		return mua.EndWithError(err)
	}
	defer chkFile.Close()

	var chkData vangogh_local_data.ValidationFile
	if err := xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return mua.EndWithError(err)
	}

	sourceFile, err := os.Open(absLocalFile)
	if err != nil {
		return mua.EndWithError(err)
	}
	defer sourceFile.Close()

	h := md5.New()

	stat, err := sourceFile.Stat()
	if err != nil {
		return mua.EndWithError(err)
	}

	_, filename := filepath.Split(localFile)
	vlfa := nod.NewProgress(" - %s", filename)

	vlfa.Total(uint64(stat.Size()))

	if err := dolo.CopyWithProgress(h, sourceFile, vlfa); err != nil {
		return mua.EndWithError(err)
	}

	sourceFileMD5 := fmt.Sprintf("%x", h.Sum(nil))

	if chkData.MD5 != sourceFileMD5 {
		vlfa.EndWithResult("error")
		return ErrValidationFailed
	} else {
		vlfa.EndWithResult("valid")
	}

	return nil
}

type validateDelegate struct {
	rdx                  kevlar.WriteableRedux
	validated            map[string]bool
	unresolvedManualUrl  map[string]bool
	missingDownloads     map[string]bool
	missingChecksum      map[string]bool
	missingExtraChecksum map[string]bool
	failed               map[string]bool
	slugLastError        map[string]string
}

func (vd *validateDelegate) Process(id string, slug string, list vangogh_local_data.DownloadsList) error {

	sva := nod.Begin(slug)
	defer sva.End()

	if vd.validated == nil {
		vd.validated = make(map[string]bool)
	}
	if vd.unresolvedManualUrl == nil {
		vd.unresolvedManualUrl = make(map[string]bool)
	}
	if vd.missingDownloads == nil {
		vd.missingDownloads = make(map[string]bool)
	}
	if vd.missingChecksum == nil {
		vd.missingChecksum = make(map[string]bool)
	}
	if vd.missingExtraChecksum == nil {
		vd.missingExtraChecksum = make(map[string]bool)
	}
	if vd.failed == nil {
		vd.failed = make(map[string]bool)
	}
	if vd.slugLastError == nil {
		vd.slugLastError = make(map[string]string)
	}

	hasValidationTargets := false
	results := make([]string, 0, 1)

	for _, dl := range list {
		if err := validateManualUrl(&dl, vd.rdx); errors.Is(err, ErrValidationNotSupported) {
			continue
		} else if errors.Is(err, ErrMissingChecksum) {
			if dl.Type == vangogh_local_data.Extra {
				vd.missingExtraChecksum[slug] = true
			} else {
				vd.missingChecksum[slug] = true
				results = append(results, "missing-checksum")
			}
		} else if errors.Is(err, ErrUnresolvedManualUrl) {
			vd.unresolvedManualUrl[slug] = true
			results = append(results, "unresolved-manual-url")
		} else if errors.Is(err, ErrMissingDownload) {
			vd.missingDownloads[slug] = true
			results = append(results, "missing-download")
		} else if errors.Is(err, ErrValidationFailed) {
			vd.failed[slug] = true
			results = append(results, "failed-validation")
		} else if err != nil {
			results = append(results, err.Error())
			vd.slugLastError[slug] = err.Error()
			continue
		}
		// don't attempt to assess success for files that don't support validation
		hasValidationTargets = true
	}

	//now := strconv.FormatInt(time.Now().UTC().Unix(), 10)
	//if err := vd.rdx.ReplaceValues(vangogh_local_data.ValidationCompletedProperty, id, now); err != nil {
	//	return err
	//}

	if hasValidationTargets &&
		!vd.missingChecksum[slug] &&
		!vd.unresolvedManualUrl[slug] &&
		!vd.missingDownloads[slug] &&
		!vd.failed[slug] &&
		vd.slugLastError[slug] == "" {
		vd.validated[slug] = true
		results = append(results, "OK")
	}

	//if err := vd.rdx.ReplaceValues(vangogh_local_data.ValidationResultProperty, id, results...); err != nil {
	//	return err
	//}

	return nil
}

func validateUpdated(since int64,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	excludePatches bool) error {

	vrAccountProducts, err := vangogh_local_data.NewProductReader(vangogh_local_data.AccountProducts)
	if err != nil {
		return err
	}

	var ids []string
	updatedAfter, err := vrAccountProducts.CreatedOrUpdatedAfter(since)
	if err != nil {
		return err
	}
	ids = append(ids, updatedAfter...)

	return Validate(ids, operatingSystems, langCodes, downloadTypes, excludePatches, false, false)
}
