package rest

import (
	"net/http"
	"os"
	"path/filepath"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wine_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetWineBinaryFile(w http.ResponseWriter, r *http.Request) {

	// GET /api/wine-binary-file?title&os

	q := r.URL.Query()

	operatingSystem := vangogh_integration.ParseOperatingSystem(q.Get(vangogh_integration.OperatingSystemsProperty))
	title := q.Get(vangogh_integration.TitleProperty)

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

	wineBinariesDir := vangogh_integration.Pwd.AbsRelDirPath(vangogh_integration.WineBinaries, vangogh_integration.Downloads)
	gitHubReleasesDir := vangogh_integration.Pwd.AbsRelDirPath(vangogh_integration.GitHubReleases, vangogh_integration.Metadata)

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
