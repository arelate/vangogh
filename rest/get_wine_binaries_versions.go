package rest

import (
	"encoding/json"
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
)

func GetWineBinariesVersions(w http.ResponseWriter, r *http.Request) {

	binariesVersions := make(map[string]string)

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

	for _, binaries := range vangogh_integration.OsWineBinaries {
		for _, binary := range binaries {
			switch binary.Version {
			case "":
				latestRelease, err := github_integration.GetLatestRelease(binary.GitHubOwnerRepo, kvGitHubReleases)
				if err != nil {
					http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
					return
				}
				binariesVersions[binary.GitHubOwnerRepo] = latestRelease.TagName
			default:
				binariesVersions[binary.Title] = binary.Version
			}
		}
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	if err = json.NewEncoder(w).Encode(binariesVersions); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
