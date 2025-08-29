package rest

import (
	"encoding/json"
	"net/http"
	"path"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wine_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
)

func GetWineBinariesVersions(w http.ResponseWriter, r *http.Request) {

	// GET /api/wine-binaries-versions

	binariesVersions := make([]vangogh_integration.WineBinaryDetails, 0, len(wine_integration.OsWineBinaries))

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

	for _, binary := range wine_integration.OsWineBinaries {

		var binaryVersion string
		binaryVersion, err = binary.GetVersion(kvGitHubReleases)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		var binaryDigest string
		binaryDigest, err = binary.GetDigest(kvGitHubReleases)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		var binaryDownloadUrl string
		binaryDownloadUrl, err = binary.GetDownloadUrl(kvGitHubReleases)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		wbd := vangogh_integration.WineBinaryDetails{
			Title:    binary.String(),
			OS:       binary.OS,
			Version:  binaryVersion,
			Digest:   binaryDigest,
			Filename: path.Base(binaryDownloadUrl),
		}

		binariesVersions = append(binariesVersions, wbd)
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	if err = json.NewEncoder(w).Encode(binariesVersions); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
