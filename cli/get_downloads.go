package cli

import (
	"errors"
	"iter"
	"net/http"
	"net/url"
	"os"
	"path"
	"path/filepath"
	"slices"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

type getDownloadOptions struct {
	updateDetails bool
	validate      bool
	checksumsOnly bool
	missing       bool
	all           bool
	debug         bool
	force         bool
}

func GetDownloadsHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	q := u.Query()
	var manualUrlFilter []string

	if q.Has("manual-url-filter") {
		manualUrlFilter = strings.Split(q.Get("manual-url-filter"), ",")
	}

	gdo := &getDownloadOptions{
		updateDetails: q.Has("update-details"),
		validate:      q.Has("validate"),
		checksumsOnly: q.Has("checksums-only"),
		missing:       q.Has("missing"),
		all:           q.Has("all"),
		debug:         q.Has("debug"),
		force:         q.Has("force"),
	}

	return GetDownloads(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		q.Has("no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		gdo,
		manualUrlFilter...)
}

func GetDownloads(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	options *getDownloadOptions,
	manualUrlFilter ...string) error {

	gda := nod.NewProgress("downloading product files...")
	defer gda.Done()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	hc, err := gogAuthHttpClient()
	if err != nil {
		return err
	}

	if err = gog_integration.IsLoggedIn(hc); err != nil {
		return err
	}

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		append(
			vangogh_integration.DownloadsLifecycleProperties(),
			vangogh_integration.SlugProperty,
			vangogh_integration.ProductTypeProperty,
			// for optional validations
			vangogh_integration.ManualUrlFilenameProperty,
			vangogh_integration.ManualUrlStatusProperty,
			vangogh_integration.ManualUrlValidationResultProperty,
			vangogh_integration.ProductValidationResultProperty)...)
	if err != nil {
		return err
	}

	if options.missing {
		missingIds, err := itemizations.MissingLocalDownloads(
			rdx,
			operatingSystems,
			downloadTypes,
			langCodes,
			noPatches,
			downloadsLayout,
			options.debug)
		if err != nil {
			return err
		}

		if len(missingIds) == 0 {
			gda.EndWithResult("all downloads are available locally")
			return nil
		}

		ids = append(ids, missingIds...)
	}

	if options.all {
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

	if options.updateDetails {
		if err = GetData(ids,
			[]vangogh_integration.ProductType{vangogh_integration.Details},
			-1,
			false,
			false,
			true); err != nil {
			return err
		}
	}

	gdd := &getDownloadsDelegate{
		rdx:             rdx,
		forceUpdate:     options.force,
		downloadsLayout: downloadsLayout,
		checksumsOnly:   options.checksumsOnly,
		manualUrlFilter: manualUrlFilter,
	}

	if options.validate {
		gdd.validateDelegate = &validateDelegate{
			rdx:             rdx,
			downloadsLayout: downloadsLayout,
			statuses:        make(map[vangogh_integration.ValidationStatus]int),
		}
	}

	if err = vangogh_integration.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		gdd,
		gda); err != nil {
		return err
	}

	return nil
}

type getDownloadsDelegate struct {
	rdx              redux.Writeable
	forceUpdate      bool
	downloadsLayout  vangogh_integration.DownloadsLayout
	checksumsOnly    bool
	manualUrlFilter  []string
	validateDelegate *validateDelegate
}

func (gdd *getDownloadsDelegate) Process(id, slug string, list vangogh_integration.DownloadsList) error {
	sda := nod.Begin("downloading %s...", slug)
	defer sda.Done()

	if len(list) == 0 {
		sda.EndWithResult("no downloads for requested operating systems, download types, languages")
		// if there's nothing to download - remove download from the queue, or it'll be stuck there
		return gdd.rdx.CutKeys(vangogh_integration.DownloadQueuedProperty, id)
	}

	manualUrls := make([]string, 0, len(list))
	for _, dl := range list {
		if len(gdd.manualUrlFilter) > 0 && !slices.Contains(gdd.manualUrlFilter, dl.ManualUrl) {
			continue
		}
		manualUrls = append(manualUrls, dl.ManualUrl)
	}

	// clear local manual URLs prior to downloading when forcing updates
	// to avoid using stale manualUrls if something goes wrong - we either get all updated values
	// or some values stay unresolved - indicating problem with the last download operation
	if gdd.forceUpdate {
		if err := gdd.rdx.CutKeys(vangogh_integration.ManualUrlFilenameProperty, manualUrls...); err != nil {
			return err
		}
	}

	// reset manual-urls download status to queued prior to downloading
	// reset manual-urls validation result to unknown prior to downloading
	// reset manual-urls generated checksums prior to downloading
	manualUrlDq := make(map[string][]string)
	manualUrlVsu := make(map[string][]string)
	for _, manualUrl := range manualUrls {
		manualUrlDq[manualUrl] = []string{vangogh_integration.DownloadStatusQueued.String()}
		manualUrlVsu[manualUrl] = []string{vangogh_integration.ValidationStatusUnknown.String()}
	}

	if err := gdd.rdx.BatchReplaceValues(vangogh_integration.ManualUrlStatusProperty, manualUrlDq); err != nil {
		return err
	}

	if err := gdd.rdx.BatchReplaceValues(vangogh_integration.ManualUrlValidationResultProperty, manualUrlVsu); err != nil {
		return err
	}

	if err := gdd.rdx.CutKeys(vangogh_integration.ManualUrlGeneratedChecksumProperty, manualUrls...); err != nil {
		return err
	}

	// (re-)set product validation result prior to downloading
	// (re-)set product generated checksum prior to downloading
	if err := gdd.rdx.ReplaceValues(
		vangogh_integration.ProductValidationResultProperty,
		id,
		vangogh_integration.ValidationStatusUnknown.String()); err != nil {
		return err
	}

	if err := gdd.rdx.CutKeys(vangogh_integration.ProductGeneratedChecksumProperty, id); err != nil {
		return err
	}

	formattedNow := time.Now().UTC().Format(time.RFC3339)

	if err := gdd.rdx.ReplaceValues(vangogh_integration.DownloadStartedProperty, id, formattedNow); err != nil {
		return err
	}

	hc, err := gogAuthHttpClient()
	if err != nil {
		return err
	}

	dc := reqs.GetDoloClient()

	for _, dl := range list {
		if len(gdd.manualUrlFilter) > 0 && !slices.Contains(gdd.manualUrlFilter, dl.ManualUrl) {
			continue
		}
		if err = gdd.downloadManualUrl(slug, &dl, hc, dc); err != nil {
			sda.Error(err)
		}
	}

	formattedNow = time.Now().UTC().Format(time.RFC3339)

	if err = gdd.rdx.ReplaceValues(vangogh_integration.DownloadCompletedProperty, id, formattedNow); err != nil {
		return err
	}

	if err = gdd.rdx.CutKeys(vangogh_integration.DownloadQueuedProperty, id); err != nil {
		return err
	}

	if gdd.validateDelegate != nil {
		if err = gdd.validateDelegate.Process(id, slug, list); err != nil {
			return err
		}
	}

	return nil
}

func (gdd *getDownloadsDelegate) downloadManualUrl(
	slug string,
	dl *vangogh_integration.Download,
	httpClient *http.Client,
	dlClient *dolo.Client) error {

	dmua := nod.NewProgress(" %s:", dl.String())
	defer dmua.Done()

	//downloading a manual URL is the following set of steps:
	//0 - set the manual-url status to downloading
	//1 - check if local file exists (based on manual-url -> filename) before attempting to resolve manual-url
	//2 - resolve the source URL to an actual session URL
	//3 - set association from manualUrl to a resolved resolvedFilename
	//4 - construct local relative dir and resolvedFilename based on manualUrl type (installer, movie, dlc, extra)
	//5 - for a given set of extensions - download checksums for installers
	//6 - download authorized session URL to a file
	//7 - set the manual-url status to downloaded

	// 0
	if err := gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlStatusProperty,
		dl.ManualUrl, vangogh_integration.DownloadStatusDownloading.String()); err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	// 1
	if !gdd.forceUpdate {
		if filename, ok := gdd.rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl); ok && filename != "" {
			absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.DownloadType, gdd.downloadsLayout)
			if err != nil {
				return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
			}

			absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

			if _, err = os.Stat(absDownloadPath); err == nil {

				// set the file as Downloaded, to avoid keeping it as Downloading
				if err = gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlStatusProperty,
					dl.ManualUrl, vangogh_integration.DownloadStatusDownloaded.String()); err != nil {
					return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
				}

				dmua.EndWithResult("already exists: %s", filename)
				return nil
			} else if !os.IsNotExist(err) {
				return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
			}
		}
	}

	// 2
	resp, err := httpClient.Head(gog_integration.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	//check for error status codes and store them for the manualUrl to provide a hint that locally missing file
	//is not a problem that can be solved locally (it's a remote source error)
	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		if err = gdd.rdx.ReplaceValues(vangogh_integration.DownloadStatusErrorProperty, dl.ManualUrl, strconv.Itoa(resp.StatusCode)); err != nil {
			return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
		}
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, errors.New(resp.Status))
	}

	resolvedUrl := resp.Request.URL

	if err = resp.Body.Close(); err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	// 3
	_, resolvedFilename := path.Split(resolvedUrl.Path)
	if err = gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl, resolvedFilename); err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	// 4
	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.DownloadType, gdd.downloadsLayout)
	if err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	absDownloadPath := filepath.Join(absSlugDownloadDir, resolvedFilename)

	// 5
	if dl.DownloadType == vangogh_integration.Installer || dl.DownloadType == vangogh_integration.DLC {
		if remoteChecksumPath := resolvedUrl.Path + kevlar.XmlExt; remoteChecksumPath != "" {

			var absChecksumPath string
			absChecksumPath, err = vangogh_integration.AbsChecksumPath(absDownloadPath)
			if err != nil {
				return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
			}

			// delete existing checksum in case the origin checksum produced 404 for a silently
			// updated new version and we'd use a generated checksum to validate
			if _, err = os.Stat(absChecksumPath); err == nil && gdd.forceUpdate {
				if err = os.Remove(absChecksumPath); err != nil {
					return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
				}
			}

			_, checksumFilename := filepath.Split(absChecksumPath)

			dca := nod.NewProgress(" - %s", checksumFilename)
			originalPath := resolvedUrl.Path
			resolvedUrl.Path = remoteChecksumPath
			if err = dlClient.Download(resolvedUrl, gdd.forceUpdate, dca, absChecksumPath); err != nil {
				//don't interrupt normal download due to checksum download error,
				//so don't return this error, just log it
				dca.Error(err)
			} else {
				dca.EndWithResult("downloaded")
			}
			resolvedUrl.Path = originalPath
		}
	}

	if gdd.checksumsOnly {
		dmua.EndWithResult("downloaded checksum only")
		return nil
	}

	// 6
	lfa := nod.NewProgress(" - %s", resolvedFilename)
	defer lfa.Done()

	if err = dlClient.Download(resolvedUrl, gdd.forceUpdate, lfa, absDownloadPath); err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	// 7
	if err = gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlStatusProperty,
		dl.ManualUrl, vangogh_integration.DownloadStatusDownloaded.String()); err != nil {
		return errManualUrlDownloadInterrupted(dl.ManualUrl, gdd.rdx, err)
	}

	lfa.EndWithResult("downloaded")

	return nil
}

func getQueuedDownloads() ([]string, error) {

	gqda := nod.Begin("getting queued downloads...")
	defer gqda.Done()

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.DownloadQueuedProperty)
	if err != nil {
		return nil, err
	}

	queuedDownloads := make([]string, 0)
	for id := range rdx.Keys(vangogh_integration.DownloadQueuedProperty) {
		queuedDownloads = append(queuedDownloads, id)
	}

	if len(queuedDownloads) == 0 {
		gqda.EndWithResult("queue is empty")
	} else {
		gqda.EndWithResult("%d downloads in the queue", len(queuedDownloads))
	}

	return queuedDownloads, nil
}

func errManualUrlDownloadInterrupted(manualUrl string, rdx redux.Writeable, sourceErr error) error {

	if err := rdx.ReplaceValues(vangogh_integration.ManualUrlStatusProperty,
		manualUrl, vangogh_integration.DownloadStatusInterrupted.String()); err != nil {
		return errors.Join(err, sourceErr)
	}

	return sourceErr
}

func queueDownloads(ids iter.Seq[string]) error {
	qda := nod.Begin("adding downloads to the queue...")
	defer qda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.DownloadQueuedProperty)
	if err != nil {
		return err
	}

	downloadsQueued := make(map[string][]string)
	for id := range ids {
		downloadsQueued[id] = []string{time.Now().UTC().Format(time.RFC3339)}
	}

	return rdx.BatchReplaceValues(vangogh_integration.DownloadQueuedProperty, downloadsQueued)
}
