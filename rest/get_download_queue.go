package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetDownloadsQueue(w http.ResponseWriter, r *http.Request) {

	// GET /downloads-queue

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	var queuedIds []string
	for id := range rdx.Keys(vangogh_integration.DownloadQueuedProperty) {
		queuedIds = append(queuedIds, id)
	}

	var err error
	queuedIds, err = rdx.Sort(queuedIds, true, vangogh_integration.GOGOrderDateProperty, vangogh_integration.TitleProperty)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	permissions, err := sb.GetPermissions(r)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	dqPage := compton_pages.DownloadsQueue(queuedIds, rdx, permissions...)
	if err = dqPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

}
