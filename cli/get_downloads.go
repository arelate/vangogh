package cli

import (
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/vangogh/cli/itemizations"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"net/url"
	"os"
	"path"
	"path/filepath"
	"strconv"
)

func GetDownloadsHandler(u *url.URL) error {
	ids, err := vangogh_local_data.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return GetDownloads(
		ids,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		vangogh_local_data.FlagFromUrl(u, "missing"),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func GetDownloads(
	ids []string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	missing,
	force bool) error {

	gda := nod.NewProgress("downloading product files...")
	defer gda.End()

	vangogh_local_data.PrintParams(ids, operatingSystems, langCodes, downloadTypes, noPatches)

	acp, err := vangogh_local_data.AbsCookiePath()
	if err != nil {
		return gda.EndWithError(err)
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return gda.EndWithError(err)
	}

	li, err := gog_integration.LoggedIn(hc)
	if err != nil {
		return gda.EndWithError(err)
	}

	if !li {
		return gda.EndWithError(fmt.Errorf("user is not logged in"))
	}

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.NativeLanguageNameProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.ManualUrlStatusProperty,
		vangogh_local_data.DownloadStatusErrorProperty,
		vangogh_local_data.ProductValidationResultProperty)
	if err != nil {
		return gda.EndWithError(err)
	}

	if missing {
		missingIds, err := itemizations.MissingLocalDownloads(
			rdx,
			operatingSystems,
			downloadTypes,
			langCodes,
			noPatches)
		if err != nil {
			return gda.EndWithError(err)
		}

		if len(missingIds) == 0 {
			gda.EndWithResult("all downloads are available locally")
			return nil
		}

		ids = append(ids, missingIds...)
	}

	gdd := &getDownloadsDelegate{
		rdx:         rdx,
		forceUpdate: force,
	}

	if err := vangogh_local_data.MapDownloads(
		ids,
		rdx,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		gdd,
		gda); err != nil {
		return gda.EndWithError(err)
	}

	gda.EndWithResult("done")

	return nil
}

type getDownloadsDelegate struct {
	rdx         kevlar.WriteableRedux
	forceUpdate bool
}

func (gdd *getDownloadsDelegate) Process(id, slug string, list vangogh_local_data.DownloadsList) error {
	sda := nod.Begin(slug)
	defer sda.End()

	if len(list) == 0 {
		sda.EndWithResult("no downloads for requested operating systems, download types, languages")
		return nil
	}

	// (re-)set manual-urls status to queued prior to downloading
	manualUrlsQueued := make(map[string][]string)
	for _, dl := range list {
		manualUrlsQueued[dl.ManualUrl] = []string{vangogh_local_data.ManualUrlQueued.String()}
	}
	if err := gdd.rdx.BatchAddValues(vangogh_local_data.ManualUrlStatusProperty, manualUrlsQueued); err != nil {
		return sda.EndWithError(err)
	}

	// (re-)set product validation result prior to downloading
	if err := gdd.rdx.ReplaceValues(
		vangogh_local_data.ProductValidationResultProperty,
		id,
		vangogh_local_data.ValidationResultUnknown.String()); err != nil {
		return sda.EndWithError(err)
	}

	acp, err := vangogh_local_data.AbsCookiePath()
	if err != nil {
		return sda.EndWithError(err)
	}

	hc, err := coost.NewHttpClientFromFile(acp)
	if err != nil {
		return sda.EndWithError(err)
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
	dl *vangogh_local_data.Download,
	httpClient *http.Client,
	dlClient *dolo.Client) error {

	dmua := nod.NewProgress(" %s:", dl.String())
	defer dmua.End()

	//downloading a manual URL is the following set of steps:
	//1 - check if local file exists (based on manualUrl -> relative localFile association) before attempting to resolve manualUrl
	//2 - resolve the source URL to an actual session URL
	//3 - construct local relative dir and filename based on manualUrl type (installer, movie, dlc, extra)
	//4 - for a given set of extensions - download validation file
	//5 - download authorized session URL to a file
	//6 - set association from manualUrl to a resolved filename
	if err := gdd.rdx.MustHave(
		vangogh_local_data.LocalManualUrlProperty,
		vangogh_local_data.DownloadStatusErrorProperty); err != nil {
		return dmua.EndWithError(err)
	}

	//1
	if !gdd.forceUpdate {
		if localPath, ok := gdd.rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl); ok {
			//localFilename would be a relative path for a download - s/slug,
			//and RelToAbs would convert this to downloads/s/slug
			addp, err := vangogh_local_data.AbsDownloadDirFromRel(localPath)
			if err != nil {
				return dmua.EndWithError(err)
			}
			if _, err := os.Stat(addp); err == nil {
				_, localFilename := filepath.Split(localPath)
				lfa := nod.Begin(" - %s", localFilename)
				lfa.EndWithResult("already exists")
				return nil
			} else if !os.IsNotExist(err) {
				return dmua.EndWithError(err)
			}
		}
	}

	//2
	resp, err := httpClient.Head(gog_integration.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		return dmua.EndWithError(err)
	}
	//check for error status codes and store them for the manualUrl to provide a hint that locally missing file
	//is not a problem that can be solved locally (it's a remote source error)
	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		if err := gdd.rdx.ReplaceValues(vangogh_local_data.DownloadStatusErrorProperty, dl.ManualUrl, strconv.Itoa(resp.StatusCode)); err != nil {
			return dmua.EndWithError(err)
		}
		return dmua.EndWithError(fmt.Errorf(resp.Status))
	}

	resolvedUrl := resp.Request.URL

	if err := resp.Body.Close(); err != nil {
		return dmua.EndWithError(err)
	}

	//3
	_, filename := path.Split(resolvedUrl.Path)
	//ProductDownloadsAbsDir would return absolute dir path, e.g. downloads/s/slug
	absDir, err := vangogh_local_data.AbsProductDownloadsDir(slug)
	if err != nil {
		return dmua.EndWithError(err)
	}
	//we need to add suffix to a dir path, e.g. dlc, extras
	relDirSuffix := ""
	switch dl.Type {
	case vangogh_local_data.DLC:
		relDirSuffix, err = pathways.GetRelDir(vangogh_local_data.DLCs)
	case vangogh_local_data.Extra:
		relDirSuffix, err = pathways.GetRelDir(vangogh_local_data.Extras)
	default:
		// do nothing - use base product downloads dir
	}
	if err != nil {
		return dmua.EndWithError(err)
	}

	//completing absDir with download type relative suffix (e.g. /g/game + dlc = /g/game/dlc)
	absDir = filepath.Join(absDir, relDirSuffix)

	//4
	remoteChecksumPath := vangogh_local_data.RemoteChecksumPath(resolvedUrl.Path)
	if remoteChecksumPath != "" {
		localChecksumPath, err := vangogh_local_data.AbsLocalChecksumPath(path.Join(absDir, filename))
		if err != nil {
			return dmua.EndWithError(err)
		}
		checksumDir, checksumFilename := filepath.Split(localChecksumPath)
		// delete existing checksum in case the origin checksum produced 404 for a silently
		// updated new version and we'd use a generated checksum to validate
		if _, err := os.Stat(localChecksumPath); err == nil && gdd.forceUpdate {
			if err := os.Remove(localChecksumPath); err != nil {
				return dmua.EndWithError(err)
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
	defer lfa.End()
	if err := dlClient.Download(resolvedUrl, gdd.forceUpdate, lfa, absDir, filename); err != nil {
		return dmua.EndWithError(err)
	}

	if err := gdd.rdx.ReplaceValues(vangogh_local_data.ManualUrlStatusProperty,
		dl.ManualUrl, vangogh_local_data.ManualUrlDownloaded.String()); err != nil {
		return dmua.EndWithError(err)
	}

	lfa.EndWithResult("downloaded")

	//6
	//ProductDownloadsRelDir would return relative (to downloads/ root) dir path, e.g. s/slug
	pRelDir, err := vangogh_local_data.RelProductDownloadsDir(slug)
	//we need to add suffix to a dir path, e.g. dlc, extras - using already resolved download type relative dir
	relDir := filepath.Join(pRelDir, relDirSuffix)
	if err != nil {
		return dmua.EndWithError(err)
	}
	//store association for ManualUrl (/downloads/en0installer) to local file (s/slug/local_filename)
	if err := gdd.rdx.ReplaceValues(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl, path.Join(relDir, filename)); err != nil {
		return dmua.EndWithError(err)
	}

	//dmua.EndWithResult("done")

	return nil
}
