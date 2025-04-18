package cli

import (
	"errors"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"net/http"
	"net/url"
	"os"
	"path"
	"path/filepath"
	"strconv"
	"time"
)

func GetDownloadsHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetDownloads(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "missing"),
		vangogh_integration.FlagFromUrl(u, "force"))
}

func GetDownloads(
	ids []string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	missing,
	force bool) error {

	gda := nod.NewProgress("downloading product files...")
	defer gda.Done()

	vangogh_integration.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return err
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return err
	}

	if err = gog_integration.IsLoggedIn(hc); err != nil {
		return err
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.SlugProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.ManualUrlFilenameProperty,
		vangogh_integration.ManualUrlStatusProperty,
		vangogh_integration.DownloadStatusErrorProperty,
		vangogh_integration.ProductValidationResultProperty,
		vangogh_integration.DownloadQueuedProperty)
	if err != nil {
		return err
	}

	if missing {
		missingIds, err := itemizations.MissingLocalDownloads(
			rdx,
			operatingSystems,
			downloadTypes,
			langCodes,
			noPatches,
			downloadsLayout)
		if err != nil {
			return err
		}

		if len(missingIds) == 0 {
			gda.EndWithResult("all downloads are available locally")
			return nil
		}

		ids = append(ids, missingIds...)
	}

	gdd := &getDownloadsDelegate{
		rdx:             rdx,
		forceUpdate:     force,
		downloadsLayout: downloadsLayout,
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
	rdx             redux.Writeable
	forceUpdate     bool
	downloadsLayout vangogh_integration.DownloadsLayout
}

func (gdd *getDownloadsDelegate) Process(id, slug string, list vangogh_integration.DownloadsList) error {
	sda := nod.Begin(slug)
	defer sda.Done()

	if len(list) == 0 {
		sda.EndWithResult("no downloads for requested operating systems, download types, languages")
		return nil
	}

	manualUrls := make([]string, 0, len(list))
	for _, dl := range list {
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

	// reset manual-urls status to queued prior to downloading
	manualUrlsQueued := make(map[string][]string)
	for _, manualUrl := range manualUrls {
		manualUrlsQueued[manualUrl] = []string{vangogh_integration.ManualUrlQueued.String()}
	}
	if err := gdd.rdx.BatchAddValues(vangogh_integration.ManualUrlStatusProperty, manualUrlsQueued); err != nil {
		return err
	}

	// (re-)set product validation result prior to downloading
	if err := gdd.rdx.ReplaceValues(
		vangogh_integration.ProductValidationResultProperty,
		id,
		vangogh_integration.ValidationResultUnknown.String()); err != nil {
		return err
	}

	acp, err := vangogh_integration.AbsCookiePath()
	if err != nil {
		return err
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return err
	}

	dc := reqs.GetDoloClient()

	for _, dl := range list {
		if err := gdd.downloadManualUrl(slug, &dl, hc, dc); err != nil {
			sda.Error(err)
		}
	}

	if err = gdd.rdx.CutKeys(vangogh_integration.DownloadQueuedProperty, id); err != nil {
		return err
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
	//1 - check if local file exists (based on manual-url -> filename) before attempting to resolve manual-url
	//2 - resolve the source URL to an actual session URL
	//3 - construct local relative dir and resolvedFilename based on manualUrl type (installer, movie, dlc, extra)
	//4 - for a given set of extensions - download validation file for installers
	//5 - download authorized session URL to a file
	//6 - set association from manualUrl to a resolved resolvedFilename

	//1
	if !gdd.forceUpdate {
		if filename, ok := gdd.rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl); ok && filename != "" {
			absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.Type, gdd.downloadsLayout)
			if err != nil {
				return err
			}

			absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

			if _, err := os.Stat(absDownloadPath); err == nil {
				dmua.EndWithResult("already exists: %s", filename)
				return nil
			} else if !os.IsNotExist(err) {
				return err
			}
		}
	}

	//2
	resp, err := httpClient.Head(gog_integration.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		return err
	}

	//check for error status codes and store them for the manualUrl to provide a hint that locally missing file
	//is not a problem that can be solved locally (it's a remote source error)
	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		if err := gdd.rdx.ReplaceValues(vangogh_integration.DownloadStatusErrorProperty, dl.ManualUrl, strconv.Itoa(resp.StatusCode)); err != nil {
			return err
		}
		return errors.New(resp.Status)
	}

	resolvedUrl := resp.Request.URL

	if err := resp.Body.Close(); err != nil {
		return err
	}

	//3
	_, resolvedFilename := path.Split(resolvedUrl.Path)
	if err := gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl, resolvedFilename); err != nil {
		return err
	}

	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.Type, gdd.downloadsLayout)
	if err != nil {
		return err
	}

	absDownloadPath := filepath.Join(absSlugDownloadDir, resolvedFilename)

	//4
	if dl.Type == vangogh_integration.Installer {
		if remoteChecksumPath := resolvedUrl.Path + kevlar.XmlExt; remoteChecksumPath != "" {

			absChecksumPath, err := vangogh_integration.AbsChecksumPath(absDownloadPath)
			if err != nil {
				return err
			}

			// delete existing checksum in case the origin checksum produced 404 for a silently
			// updated new version and we'd use a generated checksum to validate
			if _, err := os.Stat(absChecksumPath); err == nil && gdd.forceUpdate {
				if err := os.Remove(absChecksumPath); err != nil {
					return err
				}
			}

			_, checksumFilename := filepath.Split(absChecksumPath)

			dca := nod.NewProgress(" - %s", checksumFilename)
			originalPath := resolvedUrl.Path
			resolvedUrl.Path = remoteChecksumPath
			if err := dlClient.Download(resolvedUrl, gdd.forceUpdate, dca, absChecksumPath); err != nil {
				//don't interrupt normal download due to checksum download error,
				//so don't return this error, just log it
				dca.Error(err)
			} else {
				dca.EndWithResult("downloaded")
			}
			resolvedUrl.Path = originalPath
		}
	}

	//5
	lfa := nod.NewProgress(" - %s", resolvedFilename)
	defer lfa.Done()

	if err = dlClient.Download(resolvedUrl, gdd.forceUpdate, lfa, absDownloadPath); err != nil {
		return err
	}

	if err = gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlStatusProperty,
		dl.ManualUrl, vangogh_integration.ManualUrlDownloaded.String()); err != nil {
		return err
	}

	lfa.EndWithResult("downloaded")

	return nil
}

func getQueuedDownloads() ([]string, error) {

	gqda := nod.Begin("getting queued downloads...")
	defer gqda.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.DownloadQueuedProperty)
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

func queueDownloads(ids iter.Seq[string]) error {
	qda := nod.Begin("adding downloads to the queue...")
	defer qda.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.DownloadQueuedProperty)
	if err != nil {
		return err
	}

	downloadsQueued := make(map[string][]string)
	for id := range ids {
		downloadsQueued[id] = []string{time.Now().UTC().Format(time.RFC3339)}
	}

	return rdx.BatchReplaceValues(vangogh_integration.DownloadQueuedProperty, downloadsQueued)
}
