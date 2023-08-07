package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetTitles(w http.ResponseWriter, r *http.Request) {

	// GET /titles?id&format

	ids, err := vangogh_local_data.IdSetFromUrl(r.URL)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	if err := RefreshReduxAssets(vangogh_local_data.TitleProperty); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	values := make(map[string]map[string][]string, len(ids))

	for id := range ids {
		values[id] = make(map[string][]string)
		values[id][vangogh_local_data.TitleProperty], _ = rxa.GetAllValues(vangogh_local_data.TitleProperty, id)
	}

	if err := encode(values, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
