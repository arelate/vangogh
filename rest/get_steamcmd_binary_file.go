package rest

import (
	"net/http"
	"os"
	"path/filepath"

	"github.com/arelate/southern_light/steamcmd"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func GetSteamCmdBinaryFile(w http.ResponseWriter, r *http.Request) {

	// GET /api/steamcmd-binary-file?os
	q := r.URL.Query()

	operatingSystem := vangogh_integration.ParseOperatingSystem(q.Get(vangogh_integration.OperatingSystemsProperty))
	steamCmdUrl := steamcmd.Urls[operatingSystem]
	filename := filepath.Base(steamCmdUrl)

	steamBinariesDir := vangogh_integration.Pwd.AbsRelDirPath(vangogh_integration.SteamCmdBinaries, vangogh_integration.Downloads)

	absFilepath := filepath.Join(steamBinariesDir, filename)

	if _, err := os.Stat(absFilepath); err == nil {
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
