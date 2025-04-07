package cli

import (
	"errors"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"net/url"
	"os"
	"path"
	"path/filepath"
	"strconv"
)

func GetDownloadsHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetDownloads(
		ids,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
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

	rdx, err := vangogh_integration.NewReduxWriter(
		vangogh_integration.SlugProperty,
		vangogh_integration.LocalManualUrlProperty,
		vangogh_integration.ManualUrlStatusProperty,
		vangogh_integration.DownloadStatusErrorProperty,
		vangogh_integration.ProductValidationResultProperty)
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

	if err := vangogh_integration.MapDownloads(
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
		if err := gdd.rdx.CutKeys(vangogh_integration.LocalManualUrlProperty, manualUrls...); err != nil {
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

	//there is no need to use internal httpClient with cookie support for downloading
	//manual downloads, so we're going to rely on default http.Client
	defaultClient := http.DefaultClient
	dlClient := dolo.NewClient(defaultClient, dolo.Defaults())

	for _, dl := range list {
		if err := gdd.downloadManualUrl(slug, &dl, hc, dlClient); err != nil {
			sda.Error(err)
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
	//1 - check if local file exists (based on manualUrl -> relative localFile association) before attempting to resolve manualUrl
	//2 - resolve the source URL to an actual session URL
	//3 - construct local relative dir and filename based on manualUrl type (installer, movie, dlc, extra)
	//4 - for a given set of extensions - download validation file
	//5 - download authorized session URL to a file
	//6 - set association from manualUrl to a resolved filename
	if err := gdd.rdx.MustHave(
		vangogh_integration.LocalManualUrlProperty,
		vangogh_integration.DownloadStatusErrorProperty); err != nil {
		return err
	}

	//1
	if !gdd.forceUpdate {
		if localPath, ok := gdd.rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl); ok {
			//localFilename would be a relative path for a download - s/slug,
			//and RelToAbs would convert this to downloads/s/slug
			addp, err := vangogh_integration.AbsDownloadDirFromRel(localPath)
			if err != nil {
				return err
			}
			if _, err := os.Stat(addp); err == nil {
				_, localFilename := filepath.Split(localPath)
				lfa := nod.Begin(" - %s", localFilename)
				lfa.EndWithResult("already exists")
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
	_, filename := path.Split(resolvedUrl.Path)
	//ProductDownloadsAbsDir would return absolute dir path, e.g. downloads/s/slug
	absDir, err := vangogh_integration.AbsProductDownloadsDir(slug, gdd.downloadsLayout)
	if err != nil {
		return err
	}
	//we need to add suffix to a dir path, e.g. dlc, extras
	relDirSuffix := ""
	switch dl.Type {
	case vangogh_integration.DLC:
		relDirSuffix, err = pathways.GetRelDir(vangogh_integration.DLCs)
	case vangogh_integration.Extra:
		relDirSuffix, err = pathways.GetRelDir(vangogh_integration.Extras)
	default:
		// do nothing - use base product downloads dir
	}
	if err != nil {
		return err
	}

	//completing absDir with download type relative suffix (e.g. /g/game + dlc = /g/game/dlc)
	absDir = filepath.Join(absDir, relDirSuffix)

	//4
	remoteChecksumPath := vangogh_integration.RemoteChecksumPath(resolvedUrl.Path)
	if remoteChecksumPath != "" {
		localChecksumPath, err := vangogh_integration.AbsLocalChecksumPath(path.Join(absDir, filename))
		if err != nil {
			return err
		}
		checksumDir, checksumFilename := filepath.Split(localChecksumPath)
		// delete existing checksum in case the origin checksum produced 404 for a silently
		// updated new version and we'd use a generated checksum to validate
		if _, err := os.Stat(localChecksumPath); err == nil && gdd.forceUpdate {
			if err := os.Remove(localChecksumPath); err != nil {
				return err
			}
		}
		dca := nod.NewProgress(" - %s", checksumFilename)
		originalPath := resolvedUrl.Path
		resolvedUrl.Path = remoteChecksumPath
		if err := dlClient.Download(resolvedUrl, gdd.forceUpdate, dca, checksumDir, checksumFilename); err != nil {
			//don't interrupt normal download due to checksum download error,
			//so don't return this error, just log it
			dca.Error(err)
		} else {
			dca.EndWithResult("downloaded")
		}
		resolvedUrl.Path = originalPath
	}

	//5
	lfa := nod.NewProgress(" - %s", filename)
	defer lfa.Done()
	if err := dlClient.Download(resolvedUrl, gdd.forceUpdate, lfa, absDir, filename); err != nil {
		return err
	}

	if err := gdd.rdx.ReplaceValues(vangogh_integration.ManualUrlStatusProperty,
		dl.ManualUrl, vangogh_integration.ManualUrlDownloaded.String()); err != nil {
		return err
	}

	lfa.EndWithResult("downloaded")

	//6
	//ProductDownloadsRelDir would return relative (to downloads/ root) dir path, e.g. s/slug
	pRelDir, err := vangogh_integration.RelProductDownloadsDir(slug, gdd.downloadsLayout)
	//we need to add suffix to a dir path, e.g. dlc, extras - using already resolved download type relative dir
	relDir := filepath.Join(pRelDir, relDirSuffix)
	if err != nil {
		return err
	}
	//store association for ManualUrl (/downloads/en0installer) to local file (s/slug/local_filename)
	if err := gdd.rdx.ReplaceValues(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl, path.Join(relDir, filename)); err != nil {
		return err
	}

	return nil
}
