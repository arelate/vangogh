package rest

import (
	"encoding/json"
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"path/filepath"
)

func GetWineBinariesVersions(w http.ResponseWriter, r *http.Request) {

	binariesVersions := make([]vangogh_integration.WineBinaryDetails, 0, len(vangogh_integration.OsWineBinaries))

	gitHubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	for operatingSystem, binaries := range vangogh_integration.OsWineBinaries {
		for _, binary := range binaries {

			var wbd vangogh_integration.WineBinaryDetails

			switch binary.Version {
			case "":
				var latestRelease *github_integration.GitHubRelease
				latestRelease, err = github_integration.GetLatestRelease(binary.GitHubOwnerRepo, kvGitHubReleases)
				if err != nil {
					http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
					return
				}

				latestAsset := github_integration.GetReleaseAsset(latestRelease, binary.GitHubAssetGlob)
				_, filename := filepath.Split(latestAsset.BrowserDownloadUrl)

				wbd = vangogh_integration.WineBinaryDetails{
					Title:    binary.GitHubOwnerRepo,
					OS:       operatingSystem,
					Version:  latestRelease.TagName,
					Filename: filename,
				}

				if latestAsset.Digest != nil {
					wbd.Digest = *latestAsset.Digest
				}

			default:

				_, filename := filepath.Split(binary.DownloadUrl)

				wbd = vangogh_integration.WineBinaryDetails{
					Title:    binary.Title,
					OS:       operatingSystem,
					Version:  binary.Version,
					Digest:   binary.Digest,
					Filename: filename,
				}
			}

			binariesVersions = append(binariesVersions, wbd)
		}
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	if err = json.NewEncoder(w).Encode(binariesVersions); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
