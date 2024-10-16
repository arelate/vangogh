package rest_vg

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetHasData(w http.ResponseWriter, r *http.Request) {

	// GET /has_data?product-type&id&format

	pts := vangogh_local_data.ValuesFromUrl(r.URL, "product-type")
	ids, err := vangogh_local_data.IdSetFromUrl(r.URL)

	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	values := make(map[string]map[string]string, len(pts))

	for _, pt := range pts {

		values[pt] = make(map[string]string, len(ids))

		for id := range ids {
			if ok := rdx.HasValue(vangogh_local_data.TypesProperty, id, pt); ok {
				values[pt][id] = "true"
			} else {
				values[pt][id] = "false"
			}
		}

	}

	if err := encode(values, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
