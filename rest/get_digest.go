package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/http"
	"sort"
)

func PropertyValuesCounts(rdx kvas.ReadableRedux, property string) map[string]int {
	distValues := make(map[string]int)

	for _, id := range rdx.Keys(property) {
		values, ok := rdx.GetAllValues(property, id)
		if !ok || len(values) == 0 {
			continue
		}

		for _, val := range values {
			if val == "" {
				continue
			}
			distValues[val] = distValues[val] + 1
		}
	}

	return distValues
}

func GetDigest(w http.ResponseWriter, r *http.Request) {

	// GET /digest?property&format

	properties := vangogh_local_data.PropertiesFromUrl(r.URL)

	if err := RefreshReduxAssets(properties...); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	propertyValues := make(map[string][]string)

	for _, p := range properties {
		pvc := PropertyValuesCounts(rdx, p)
		values := maps.Keys(pvc)
		sort.Strings(values)
		propertyValues[p] = values
	}

	if err := encode(propertyValues, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
