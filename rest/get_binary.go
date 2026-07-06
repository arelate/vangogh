package rest

import (
	"net/http"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/steamcmd"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wine_integration"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetBinary(w http.ResponseWriter, r *http.Request) {

	// GET /binary/{os}/{title...}

	operatingSystem := vangogh_integration.ParseOperatingSystem(r.PathValue("os"))
	title := r.PathValue("title")

	var absFilepath string

	switch title {
	case steamcmd.Title:
		steamCmdUrl := steamcmd.Urls[operatingSystem]
		filename := filepath.Base(steamCmdUrl)

		releasesDir := camino.GetRel(vangogh_integration.Releases, vangogh_integration.Binaries)

		absFilepath = filepath.Join(releasesDir, filename)
	default:
		var binary *wine_integration.Binary

		for _, bin := range wine_integration.OsWineBinaries {
			if bin.OS != operatingSystem {
				continue
			}
			if strings.ToLower(bin.String()) == strings.ToLower(title) {
				binary = &bin
			}
		}

		if binary == nil {
			http.NotFound(w, r)
			return
		}

		releasesDir := camino.GetRel(vangogh_integration.Releases, vangogh_integration.Binaries)
		gitHubReleasesDir := camino.GetRel(vangogh_integration.GitHubReleases, vangogh_integration.Metadata)

		kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		binaryUrl, err := binary.GetDownloadUrl(kvGitHubReleases)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		absFilepath = filepath.Join(releasesDir, filepath.Base(binaryUrl))
	}

	camino.ServeFile(w, r, absFilepath)
}
