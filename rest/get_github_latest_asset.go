package rest

import (
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"path/filepath"
)

func GetGitHubLatestAsset(w http.ResponseWriter, r *http.Request) {

	// GET /api/github-latest-asset?repo

	q := r.URL.Query()

	repo := q.Get("repo")

	githubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	kvGitHubReleases, err := kevlar.New(githubReleasesDir, kevlar.JsonExt)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	latestRelease, err := github_integration.GetLatestRelease(repo, kvGitHubReleases)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if asset := github_integration.GetReleaseAsset(repo, latestRelease); asset != nil {

		absAssetPath, err := vangogh_integration.AbsGitHubReleaseAssetPath(repo, latestRelease, asset)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		_, filename := filepath.Split(absAssetPath)
		w.Header().Set("Content-Disposition", "attachment; filename=\""+filename+"\"")

		http.ServeFile(w, r, absAssetPath)

	} else {
		http.NotFound(w, r)
	}

}
