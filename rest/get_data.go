package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetData(w http.ResponseWriter, r *http.Request) {

	// GET /data?product-type&id&format

	pt := vangogh_local_data.ProductTypeFromUrl(r.URL)
	ids, err := vangogh_local_data.IdSetFromUrl(r.URL)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	values := make(map[string]interface{}, len(ids))

	vr, err := vangogh_local_data.NewReader(pt)

	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	for id := range ids {
		if values[id], err = vr.ReadValue(id); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	if err := encode(values, w, r); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
