package rest

import (
	"net/http"
	"path/filepath"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/camino"
	"github.com/boggydigital/nod"
)

func GetLog(w http.ResponseWriter, r *http.Request) {

	// GET /log/{logId}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	logId := r.PathValue("logId")
	logId = filepath.Base(logId)

	switch logId {
	case "":
		http.NotFound(w, r)
	default:
		absLogPath := filepath.Join(camino.GetAbs(vangogh_integration.Logs), logId)
		camino.ServeFile(w, r, absLogPath)
	}

}
