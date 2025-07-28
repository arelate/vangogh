package cli

import (
	"crypto/sha256"
	"errors"
	"fmt"
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"net/url"
	"os"
	"path"
	"path/filepath"
	"slices"
	"strings"
	"time"
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

	start := time.Now()

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

	binariesUrls := make(map[string]*url.URL)

	for _, binary := range binaries {
		var u *url.URL
		if u, err = getWineBinaryUrl(&binary, kvGitHubReleases, force); err == nil && u != nil {
			binariesUrls[binary.String()] = u
		} else if err != nil {
			return err
		}
	}

	if err = downloadHttpWineBinaries(binariesUrls, force); err != nil {
		return err
	}

	if err = validateWineBinaries(binaries, binariesUrls, kvGitHubReleases, start, force); err != nil {
		return err
	}

	if err = cleanupWineBinaries(binariesUrls); err != nil {
		return err
	}

	return nil
}

func getWineBinaryUrl(bin *vangogh_integration.Binary, kvGitHubReleases kevlar.KeyValues, force bool) (*url.URL, error) {

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

func downloadHttpWineBinaries(urls map[string]*url.URL, force bool) error {

	dhba := nod.NewProgress("downloading binaries...")
	defer dhba.Done()

	wineBinariesDir, err := pathways.GetAbsRelDir(vangogh_integration.WineBinaries)
	if err != nil {
		return err
	}

	dhba.TotalInt(len(urls))

	for _, u := range urls {
		if err = downloadHttpWineBinary(u, wineBinariesDir, force); err != nil {
			return err
		}
		dhba.Increment()
	}

	return nil
}

func downloadHttpWineBinary(fromUrl *url.URL, toDir string, force bool) error {

	_, filename := filepath.Split(fromUrl.Path)

	ghba := nod.NewProgress(" - %s...", filename)
	defer ghba.Done()

	return dolo.DefaultClient.Download(fromUrl, force, ghba, toDir, filename)
}

func validateWineBinaries(binaries []vangogh_integration.Binary, urls map[string]*url.URL, kvGitHubReleases kevlar.KeyValues, since time.Time, force bool) error {

	vwba := nod.NewProgress("validating binaries...")
	defer vwba.Done()

	wineBinariesDir, err := pathways.GetAbsRelDir(vangogh_integration.WineBinaries)
	if err != nil {
		return err
	}

	for _, binary := range binaries {

		if u, ok := urls[binary.String()]; ok && u != nil {
			absFilename := filepath.Join(wineBinariesDir, path.Base(u.Path))
			if err = validateWineBinary(absFilename, &binary, kvGitHubReleases, since, force); err != nil {
				return err
			}
		}
	}

	return nil
}

func validateWineBinary(absFilename string, binary *vangogh_integration.Binary, kvGitHubReleases kevlar.KeyValues, since time.Time, force bool) error {

	_, filename := filepath.Split(absFilename)

	vwba := nod.NewProgress(" - %s...", filename)
	defer vwba.Done()

	stat, err := os.Stat(absFilename)
	if err != nil {
		return err
	}

	if stat.ModTime().Before(since) && !force {
		vwba.EndWithResult("unchanged")
		return nil
	}

	digest, err := binary.GetDigest(kvGitHubReleases)
	if err != nil {
		return err
	}

	if digest == "" {
		vwba.EndWithResult("digest is not available")
		return nil
	}

	wbFile, err := os.Open(absFilename)
	if err != nil {
		return err
	}

	defer wbFile.Close()

	vwba.Total(uint64(stat.Size()))

	if algorithm, dstHash, ok := strings.Cut(digest, ":"); ok {

		switch algorithm {
		case "sha256":

			h := sha256.New()
			if err = dolo.CopyWithProgress(h, wbFile, vwba); err != nil {
				return err
			}

			srcHash := fmt.Sprintf("%x", h.Sum(nil))

			if srcHash == dstHash {
				vwba.EndWithResult("valid")
				return nil
			} else {
				return errors.New("digest mismatch")
			}

		default:
			return errors.New("unknown digest algorithm: " + algorithm)
		}

	} else {
		return errors.New("unknown digest format: " + digest)
	}
}

func cleanupWineBinaries(urls map[string]*url.URL) error {

	cba := nod.Begin("cleaning up older binaries versions...")
	defer cba.Done()

	wineBinariesDir, err := pathways.GetAbsRelDir(vangogh_integration.WineBinaries)
	if err != nil {
		return err
	}

	expectedFilenames := make([]string, 0, len(urls))
	for _, u := range urls {
		_, fn := filepath.Split(u.Path)
		expectedFilenames = append(expectedFilenames, fn)
	}

	bd, err := os.Open(wineBinariesDir)
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
		absFn := filepath.Join(wineBinariesDir, fn)
		if err = os.Remove(absFn); err != nil {
			return err
		}
	}

	return nil
}
