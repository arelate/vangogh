package cli_api

import (
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_urls"
	"github.com/arelate/vangogh_downloads"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"github.com/boggydigital/vangogh/cli_api/itemize"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/http"
	"net/url"
	"os"
	"path"
	"path/filepath"
	"strconv"
)

func GetDownloadsHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	mt := gog_media.Parse(url_helpers.Value(u, "media"))

	operatingSystems := url_helpers.OperatingSystems(u)
	downloadTypes := url_helpers.DownloadTypes(u)
	langCodes := url_helpers.Values(u, "language-code")

	missing := url_helpers.Flag(u, "missing")

	forceUpdate := url_helpers.Flag(u, "force-update")

	return GetDownloads(
		idSet,
		mt,
		operatingSystems,
		downloadTypes,
		langCodes,
		missing,
		forceUpdate)
}

func GetDownloads(
	idSet gost.StrSet,
	mt gog_media.Media,
	operatingSystems []vangogh_downloads.OperatingSystem,
	downloadTypes []vangogh_downloads.DownloadType,
	langCodes []string,
	missing,
	forceUpdate bool) error {

	gda := nod.NewProgress("downloading product files...")
	defer gda.End()

	httpClient, err := http_client.Default()
	if err != nil {
		return gda.EndWithError(err)
	}

	li, err := gog_auth.LoggedIn(httpClient)
	if err != nil {
		return gda.EndWithError(err)
	}

	if !li {
		return gda.EndWithError(fmt.Errorf("user is not logged in"))
	}

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.LocalManualUrl,
		vangogh_properties.DownloadStatusError)
	if err != nil {
		return gda.EndWithError(err)
	}

	if missing {
		missingIds, err := itemize.MissingLocalDownloads(mt, exl, operatingSystems, downloadTypes, langCodes)
		if err != nil {
			return gda.EndWithError(err)
		}

		if missingIds.Len() == 0 {
			gda.EndWithResult("all downloads are available locally")
			return nil
		}

		idSet.AddSet(missingIds)
	}

	gdd := &getDownloadsDelegate{
		tpw:         gda,
		exl:         exl,
		forceUpdate: forceUpdate,
	}

	gda.TotalInt(idSet.Len())

	if err := vangogh_downloads.Map(
		idSet,
		mt,
		exl,
		operatingSystems,
		downloadTypes,
		langCodes,
		gdd); err != nil {
		return gda.EndWithError(err)
	}

	gda.EndWithResult("done")

	return nil
}

type getDownloadsDelegate struct {
	tpw         nod.TotalProgressWriter
	exl         *vangogh_extracts.ExtractsList
	forceUpdate bool
}

func (gdd *getDownloadsDelegate) Process(_, slug string, list vangogh_downloads.DownloadsList) error {
	sda := nod.Begin(slug)
	defer sda.End()

	if len(list) == 0 {
		sda.EndWithResult("no downloads for requested operating systems, download types, languages")
		return nil
	}

	httpClient, err := http_client.Default()
	if err != nil {
		return sda.EndWithError(err)
	}

	//there is no need to use internal httpClient with cookie support for downloading
	//manual downloads, so we're going to rely on default http.Client
	defaultClient := http.DefaultClient
	dlClient := dolo.NewClient(defaultClient, dolo.Defaults())

	for _, dl := range list {
		if err := gdd.downloadManualUrl(slug, &dl, httpClient, dlClient); err != nil {
			sda.Error(err)
		}
	}

	gdd.tpw.Increment()

	return nil
}

func (gdd *getDownloadsDelegate) downloadManualUrl(
	slug string,
	dl *vangogh_downloads.Download,
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
	if err := gdd.exl.AssertSupport(
		vangogh_properties.LocalManualUrl,
		vangogh_properties.DownloadStatusError); err != nil {
		return dmua.EndWithError(err)
	}

	//1
	if !gdd.forceUpdate {
		if localPath, ok := gdd.exl.Get(vangogh_properties.LocalManualUrl, dl.ManualUrl); ok {
			//localFilename would be a relative path for a download - s/slug,
			//and RelToAbs would convert this to downloads/s/slug
			if _, err := os.Stat(vangogh_urls.DownloadRelToAbs(localPath)); err == nil {
				_, localFilename := filepath.Split(localPath)
				lfa := nod.Begin(" %s", localFilename)
				lfa.EndWithResult("already exists")
				return nil
			}
		}
	}

	//2
	resp, err := httpClient.Head(gog_urls.ManualDownloadUrl(dl.ManualUrl).String())
	if err != nil {
		return dmua.EndWithError(err)
	}
	//check for error status codes and store them for the manualUrl to provide a hint that locally missing file
	//is not a problem that can be solved locally (it's a remote source error)
	if resp.StatusCode < 200 || resp.StatusCode >= 300 {
		if err := gdd.exl.Set(vangogh_properties.DownloadStatusError, dl.ManualUrl, strconv.Itoa(resp.StatusCode)); err != nil {
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
	lfa := nod.NewProgress(" - %s", filename)
	defer lfa.End()
	//ProductDownloadsAbsDir would return absolute dir path, e.g. downloads/s/slug
	pAbsDir, err := vangogh_urls.ProductDownloadsAbsDir(slug)
	if err != nil {
		return dmua.EndWithError(err)
	}
	//we need to add suffix to a dir path, e.g. dlc, extras
	absDir := filepath.Join(pAbsDir, dl.DirSuffix())

	//4
	remoteChecksumPath := vangogh_urls.RemoteChecksumPath(resolvedUrl.Path)
	if remoteChecksumPath != "" {
		localChecksumPath := vangogh_urls.LocalChecksumPath(path.Join(absDir, filename))
		if _, err := os.Stat(localChecksumPath); os.IsNotExist(err) {
			checksumDir, checksumFilename := filepath.Split(localChecksumPath)
			dca := nod.NewProgress(" - %s", checksumFilename)
			originalPath := resolvedUrl.Path
			resolvedUrl.Path = remoteChecksumPath
			if err := dlClient.Download(resolvedUrl, dca, checksumDir, checksumFilename); err != nil {
				return dca.EndWithError(err)
			}
			resolvedUrl.Path = originalPath
			dca.EndWithResult("done")
		}
	}

	//5
	if err := dlClient.Download(resolvedUrl, lfa, absDir, filename); err != nil {
		return dmua.EndWithError(err)
	}

	lfa.EndWithResult("downloaded")

	//6
	//ProductDownloadsRelDir would return relative (to downloads/ root) dir path, e.g. s/slug
	pRelDir, err := vangogh_urls.ProductDownloadsRelDir(slug)
	//we need to add suffix to a dir path, e.g. dlc, extras
	relDir := filepath.Join(pRelDir, dl.DirSuffix())
	if err != nil {
		return dmua.EndWithError(err)
	}
	//store association for ManualUrl (/downloads/en0installer) to local file (s/slug/local_filename)
	if err := gdd.exl.Set(vangogh_properties.LocalManualUrl, dl.ManualUrl, path.Join(relDir, filename)); err != nil {
		return dmua.EndWithError(err)
	}

	//dmua.EndWithResult("done")

	return nil
}
