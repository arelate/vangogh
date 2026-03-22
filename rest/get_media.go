package rest

import (
	"net/http"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetMedia(w http.ResponseWriter, r *http.Request) {

	// GET /media?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	screenshotBytes, err := compton_data.GetKeyValuesBytes(id, vangogh_integration.ScreenshotsKeyValues, keyValues)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	screenshots := strings.Split(string(screenshotBytes), ",")

	p := compton_pages.Media(id, screenshots, rdx)
	if err = p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
