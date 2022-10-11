package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetHasRedux(w http.ResponseWriter, r *http.Request) {

	// GET /has_redux?property&id&format

	properties := vangogh_local_data.PropertiesFromUrl(r.URL)

	if err := RefreshReduxAssets(properties...); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	ids, err := vangogh_local_data.IdSetFromUrl(r.URL)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	values := make(vangogh_local_data.IdReduxAssets, len(ids))
	for id := range ids {
		propValues := make(map[string][]string)
		for _, prop := range properties {
			if _, ok := rxa.GetAllValues(prop, id); ok {
				propValues[prop] = []string{"true"}
			} else {
				propValues[prop] = []string{"false"}
			}
		}
		values[id] = propValues
	}

	if err := encode(values, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
