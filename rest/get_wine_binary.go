package rest

import (
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"os"
	"path/filepath"
	"strings"
)

func GetWineBinaryFile(w http.ResponseWriter, r *http.Request) {

	// GET /api/wine-binary-file?title&os

	q := r.URL.Query()

	operatingSystem := vangogh_integration.ParseOperatingSystem(q.Get(vangogh_integration.OperatingSystemsProperty))
	title := q.Get(vangogh_integration.TitleProperty)

	var binary *vangogh_integration.Binary

	for _, bin := range vangogh_integration.OsWineBinaries[operatingSystem] {
		if strings.ToLower(bin.String()) == strings.ToLower(title) {
			binary = &bin
		}
	}

	if binary == nil {
		http.NotFound(w, r)
		return
	}

	wineBinariesDir, err := pathways.GetAbsRelDir(vangogh_integration.WineBinaries)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	binaryUrl := binary.DownloadUrl

	if binaryUrl == "" {

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

		latestRelease, err := github_integration.GetLatestRelease(binary.GitHubOwnerRepo, kvGitHubReleases)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		latestAsset := github_integration.GetReleaseAsset(latestRelease, binary.GitHubAssetGlob)

		binaryUrl = latestAsset.BrowserDownloadUrl
	}

	_, filename := filepath.Split(binaryUrl)
	absFilepath := filepath.Join(wineBinariesDir, filename)

	if _, err = os.Stat(absFilepath); err == nil {
	} else if os.IsNotExist(err) {
		http.NotFound(w, r)
		return
	} else {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Cache-Control", "max-age=31536000")
	w.Header().Set("Content-Disposition", "attachment; filename=\""+filename+"\"")

	http.ServeFile(w, r, absFilepath)
}
