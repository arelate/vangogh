package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"io"
	"net/http"
)

func GetGitHubReleases(w http.ResponseWriter, r *http.Request) {

	// GET /api/github-releases?owner-repo

	q := r.URL.Query()

	ownerRepo := q.Get("owner-repo")

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

	if !kvGitHubReleases.Has(ownerRepo) {
		http.NotFound(w, r)
	}

	rcGitHubRelease, err := kvGitHubReleases.Get(ownerRepo)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
	defer rcGitHubRelease.Close()

	if _, err = io.Copy(w, rcGitHubRelease); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

}
