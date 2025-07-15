package cli

import (
	"errors"
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"net/url"
	"os"
	"path/filepath"
	"slices"
	"strings"
)

func GetWineBinariesHandler(u *url.URL) error {

	q := u.Query()

	force := q.Has("force")

	var operatingSystems []vangogh_integration.OperatingSystem
	if q.Has(vangogh_integration.OperatingSystemsProperty) {
		operatingSystems = vangogh_integration.ParseManyOperatingSystems(
			strings.Split(q.Get(vangogh_integration.OperatingSystemsProperty), ","))
	}

	return GetWineBinaries(operatingSystems, force)
}

func GetWineBinaries(operatingSystems []vangogh_integration.OperatingSystem, force bool) error {

	gba := nod.Begin("getting WINE binaries...")
	defer gba.Done()

	if len(operatingSystems) == 0 {
		gba.EndWithResult("no operating system specified for WINE binaries")
		return nil
	}

	if slices.Contains(operatingSystems, vangogh_integration.AnyOperatingSystem) {
		operatingSystems = vangogh_integration.AllOperatingSystems()
	}

	gitHubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		return err
	}

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	binaries := make([]vangogh_integration.Binary, 0)

	for _, operatingSystem := range operatingSystems {
		binaries = append(binaries, vangogh_integration.OsWineBinaries[operatingSystem]...)
	}

	binariesUrls := make([]*url.URL, 0, len(binaries))

	for _, binary := range binaries {
		var u *url.URL
		if u, err = getBinaryUrl(&binary, kvGitHubReleases, force); err == nil && u != nil {
			binariesUrls = append(binariesUrls, u)
		} else if err != nil {
			return err
		}
	}

	if err = downloadHttpBinaries(force, binariesUrls...); err != nil {
		return err
	}

	if err = cleanupBinaries(binariesUrls...); err != nil {
		return err
	}

	return nil
}

func getBinaryUrl(bin *vangogh_integration.Binary, kvGitHubReleases kevlar.KeyValues, force bool) (*url.URL, error) {

	gba := nod.Begin(" getting %s url...", bin)
	defer gba.Done()

	switch bin.DownloadUrl {
	case "":
		return getGitHubBinaryUrl(bin.GitHubOwnerRepo, bin.GitHubAssetGlob, kvGitHubReleases, force)
	default:
		return url.Parse(bin.DownloadUrl)
	}
}

func getGitHubBinaryUrl(ownerRepo string, assetGlob string, kvGitHubReleases kevlar.KeyValues, force bool) (*url.URL, error) {

	gghba := nod.Begin(" getting %s GitHub url...", ownerRepo)
	defer gghba.Done()

	if err := getGitHubRepoReleases(ownerRepo, kvGitHubReleases); err != nil {
		return nil, err
	}

	return getGitHubLatestReleaseAssetUrl(ownerRepo, assetGlob, kvGitHubReleases)
}

func getGitHubRepoReleases(ownerRepo string, kvGitHubReleases kevlar.KeyValues) error {

	grra := nod.Begin(" getting %s releases...", ownerRepo)
	defer grra.Done()

	var owner, repo string
	if oo, rr, ok := strings.Cut(ownerRepo, "/"); ok {
		owner = oo
		repo = rr
	} else {
		grra.EndWithResult("not a valid owner/repo: %s", ownerRepo)
		return nil
	}

	ghsu := github_integration.ReleasesUrl(owner, repo)

	resp, err := http.DefaultClient.Get(ghsu.String())
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return errors.New(resp.Status)
	}

	if err = kvGitHubReleases.Set(ownerRepo, resp.Body); err != nil {
		return err
	}

	return nil
}

func getGitHubLatestReleaseAssetUrl(ownerRepo, assetGlob string, kvGitHubReleases kevlar.KeyValues) (*url.URL, error) {

	cra := nod.Begin(" getting %s latest asset url...", ownerRepo)
	defer cra.Done()

	latestRelease, err := github_integration.GetLatestRelease(ownerRepo, kvGitHubReleases)
	if err != nil {
		return nil, err
	}

	if latestRelease == nil {
		return nil, errors.New("GitHub latest release not found for " + ownerRepo)
	}

	releaseAsset := github_integration.GetReleaseAsset(latestRelease, assetGlob)

	if releaseAsset == nil {
		return nil, errors.New("GitHub latest release is missing assets matching " + assetGlob + " for " + ownerRepo)
	}

	return url.Parse(releaseAsset.BrowserDownloadUrl)
}

func downloadHttpBinaries(force bool, urls ...*url.URL) error {

	dhba := nod.NewProgress("downloading binaries...")
	defer dhba.Done()

	binariesDir, err := pathways.GetAbsRelDir(vangogh_integration.Binaries)
	if err != nil {
		return err
	}

	dhba.TotalInt(len(urls))

	for _, u := range urls {
		if err = downloadHttpBinary(u, binariesDir, force); err != nil {
			return err
		}
		dhba.Increment()
	}

	return nil
}

func downloadHttpBinary(fromUrl *url.URL, toDir string, force bool) error {

	_, filename := filepath.Split(fromUrl.Path)

	ghba := nod.NewProgress(" - %s...", filename)
	defer ghba.Done()

	return dolo.DefaultClient.Download(fromUrl, force, ghba, toDir, filename)
}

func cleanupBinaries(urls ...*url.URL) error {

	cba := nod.Begin("cleaning up older binaries versions...")
	defer cba.Done()

	binariesDir, err := pathways.GetAbsRelDir(vangogh_integration.Binaries)
	if err != nil {
		return err
	}

	expectedFilenames := make([]string, 0, len(urls))
	for _, u := range urls {
		_, fn := filepath.Split(u.Path)
		expectedFilenames = append(expectedFilenames, fn)
	}

	bd, err := os.Open(binariesDir)
	if err != nil {
		return err
	}

	actualFilenames, err := bd.Readdirnames(-1)
	if err != nil {
		return err
	}

	unexpectedFilenames := make([]string, 0)

	for _, fn := range actualFilenames {
		if !slices.Contains(expectedFilenames, fn) {
			unexpectedFilenames = append(unexpectedFilenames, fn)
		}
	}

	if len(unexpectedFilenames) == 0 {
		cba.EndWithResult("already clean")
		return nil
	}

	for _, fn := range unexpectedFilenames {
		if err = os.Remove(fn); err != nil {
			return err
		}
	}

	return nil
}
