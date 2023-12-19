package rest

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetRedux(w http.ResponseWriter, r *http.Request) {

	// GET /redux?property&id&format
	// GET /redux?property&all&format

	properties := vangogh_local_data.PropertiesFromUrl(r.URL)

	if err := RefreshReduxAssets(properties...); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	var ids map[string]bool
	var err error

	if vangogh_local_data.FlagFromUrl(r.URL, "all") {
		if len(properties) > 1 {
			err = fmt.Errorf("redux 'all' only works for a single property")
		} else if len(properties) == 0 {
			err = fmt.Errorf("redux 'all' needs at least one property")
		}
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		ids = make(map[string]bool)

		//the code above enforced a single property value for 'all' branch
		for _, id := range rdx.Keys(properties[0]) {
			ids[id] = true
		}
	} else {
		ids, err = vangogh_local_data.IdSetFromUrl(r.URL)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	values := make(kvas.IdReduxAssets, len(ids))
	for id := range ids {
		propValues := make(map[string][]string)
		for _, prop := range properties {
			propValues[prop], _ = rdx.GetAllValues(prop, id)
		}
		values[id] = propValues
	}

	if err := encode(values, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
