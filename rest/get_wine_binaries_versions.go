package rest

import (
	"encoding/json"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"net/http"
	"path"
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
				Title:    binary.GitHubOwnerRepo,
				OS:       operatingSystem,
				Version:  binaryVersion,
				Digest:   binaryDigest,
				Filename: path.Base(binaryDownloadUrl),
			}

			binariesVersions = append(binariesVersions, wbd)
		}
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	if err = json.NewEncoder(w).Encode(binariesVersions); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
