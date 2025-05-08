package cli

import (
	"encoding/json"
	"errors"
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"io/fs"
	"iter"
	"maps"
	"net/http"
	"net/url"
	"os"
	"path/filepath"
	"strings"
	"time"
)

const forceGitHubUpdatesDays = 7

func CacheGitHubReleasesHandler(u *url.URL) error {
	return CacheGitHubReleases(u.Query().Has("force"))
}

func CacheGitHubReleases(force bool) error {

	cgra := nod.Begin("caching GitHub releases...")
	defer cgra.Done()

	for _, operatingSystem := range vangogh_integration.AllOperatingSystems() {
		if operatingSystem == vangogh_integration.AnyOperatingSystem {
			continue
		}

		if err := getGitHubReleases(operatingSystem, force); err != nil {
			return err
		}

		if err := downloadGitHubLatestRelease(operatingSystem, force); err != nil {
			return err
		}

		if err := cleanupGitHubReleases(operatingSystem); err != nil {
			return err
		}
	}

	return nil
}

func getGitHubReleases(operatingSystem vangogh_integration.OperatingSystem, force bool) error {

	ggra := nod.Begin(" getting GitHub releases for %s...", operatingSystem)
	defer ggra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GitHubReleasesUpdatedProperty)
	if err != nil {
		return err
	}

	gitHubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		return err
	}

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	forceRepoUpdate := force

	for _, ownerRepo := range vangogh_integration.OperatingSystemGitHubRepos(operatingSystem) {

		if ghsu, ok := rdx.GetLastVal(vangogh_integration.GitHubReleasesUpdatedProperty, ownerRepo); ok && ghsu != "" {
			if ghsut, err := time.Parse(time.RFC3339, ghsu); err == nil {
				if ghsut.AddDate(0, 0, forceGitHubUpdatesDays).Before(time.Now()) {
					forceRepoUpdate = true
				}
			}
		}

		if err = getRepoReleases(ownerRepo, kvGitHubReleases, rdx, forceRepoUpdate); err != nil {
			return err
		}
	}

	return nil
}

func getRepoReleases(ownerRepo string, kvGitHubReleases kevlar.KeyValues, rdx redux.Writeable, force bool) error {

	grlra := nod.Begin(" %s...", ownerRepo)
	defer grlra.Done()

	if kvGitHubReleases.Has(ownerRepo) && !force {
		grlra.EndWithResult("skip recently updated")
		return nil
	}

	var owner, repo string
	if oo, rr, ok := strings.Cut(ownerRepo, "/"); ok {
		owner = oo
		repo = rr
	} else {
		grlra.EndWithResult("not a valid owner/repo: %s", ownerRepo)
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

	ft := time.Now().Format(time.RFC3339)
	return rdx.ReplaceValues(vangogh_integration.GitHubReleasesUpdatedProperty, ownerRepo, ft)
}

func downloadGitHubLatestRelease(operatingSystem vangogh_integration.OperatingSystem, force bool) error {

	cra := nod.Begin(" caching GitHub releases for %s...", operatingSystem)
	defer cra.Done()

	gitHubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		return err
	}

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	dc := dolo.DefaultClient

	for _, ownerRepo := range vangogh_integration.OperatingSystemGitHubRepos(operatingSystem) {

		latestRelease, err := github_integration.GetLatestRelease(ownerRepo, kvGitHubReleases)
		if err != nil {
			return err
		}

		if latestRelease == nil {
			continue
		}

		if err = downloadRepoRelease(ownerRepo, latestRelease, dc, force); err != nil {
			return err
		}
	}

	return nil
}

func downloadRepoRelease(ownerRepo string, release *github_integration.GitHubRelease, dc *dolo.Client, force bool) error {

	crra := nod.Begin(" - tag: %s...", release.TagName)
	defer crra.Done()

	asset := github_integration.GetReleaseAsset(ownerRepo, release)
	if asset == nil {
		crra.EndWithResult("asset not found")
		return nil
	}

	ru, err := url.Parse(asset.BrowserDownloadUrl)
	if err != nil {
		return err
	}

	relDir, err := vangogh_integration.AbsGitHubReleasesDir(ownerRepo, release)
	if err != nil {
		return err
	}

	dra := nod.NewProgress(" - asset: %s", asset.Name)
	defer dra.Done()

	if err = dc.Download(ru, force, dra, relDir); err != nil {
		return err
	}

	return nil
}

func cleanupGitHubReleases(operatingSystem vangogh_integration.OperatingSystem) error {

	cra := nod.Begin("cleaning up cached GitHub releases, keeping the latest for %s...", operatingSystem)
	defer cra.Done()

	gitHubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		return err
	}

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	for _, ownerRepo := range vangogh_integration.OperatingSystemGitHubRepos(operatingSystem) {

		if err = cleanupRepoReleases(ownerRepo, kvGitHubReleases); err != nil {
			return err
		}
	}

	return nil
}

func cleanupRepoReleases(ownerRepo string, kvGitHubReleases kevlar.KeyValues) error {
	crra := nod.Begin(" %s...", ownerRepo)
	defer crra.Done()

	rcReleases, err := kvGitHubReleases.Get(ownerRepo)
	if err != nil {
		return err
	}
	defer rcReleases.Close()

	var releases []github_integration.GitHubRelease
	if err = json.NewDecoder(rcReleases).Decode(&releases); err != nil {
		return err
	}

	cleanupFiles := make([]string, 0)

	for ii, release := range releases {
		if ii == 0 {
			continue
		}

		asset := github_integration.GetReleaseAsset(ownerRepo, &release)
		if asset == nil {
			continue
		}

		absReleaseAssetPath, err := vangogh_integration.AbsGitHubReleaseAssetPath(ownerRepo, &release, asset)
		if err != nil {
			return err
		}

		if _, err := os.Stat(absReleaseAssetPath); err == nil {
			cleanupFiles = append(cleanupFiles, absReleaseAssetPath)
		}
	}

	if len(cleanupFiles) == 0 {
		crra.EndWithResult("already clean")
		return nil
	} else {
		if err := removeRepoReleasesFiles(cleanupFiles); err != nil {
			return err
		}
	}

	return nil
}

func removeRepoReleasesFiles(absFilePaths []string) error {
	rfa := nod.NewProgress("cleaning up older releases files...")
	defer rfa.Done()

	rfa.TotalInt(len(absFilePaths))

	absDirs := make(map[string]any)

	for _, absFilePath := range absFilePaths {
		dir, _ := filepath.Split(absFilePath)
		absDirs[dir] = nil
		if err := os.Remove(absFilePath); err != nil {
			return err
		}

		rfa.Increment()
	}

	return removeRepoReleaseDirs(maps.Keys(absDirs))
}

func hasOnlyDSStore(entries []fs.DirEntry) bool {
	if len(entries) == 1 {
		return entries[0].Name() == ".DS_Store"
	}
	return false
}

func removeDirIfEmpty(dirPath string) error {
	if entries, err := os.ReadDir(dirPath); err == nil && len(entries) == 0 {
		if err := os.Remove(dirPath); err != nil {
			return err
		}
	} else if err == nil && hasOnlyDSStore(entries) {
		if err := os.RemoveAll(dirPath); err != nil {
			return err
		}
	} else if err != nil {
		return err
	}
	return nil
}

func removeRepoReleaseDirs(absDirs iter.Seq[string]) error {
	rda := nod.Begin("cleaning up older releases directories...")
	defer rda.Done()

	for absDir := range absDirs {
		if err := removeDirIfEmpty(absDir); err != nil {
			return err
		}
	}
	return nil
}
