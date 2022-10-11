package rest

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func PatchLocalTag(w http.ResponseWriter, r *http.Request) {

	// PATCH /local_tag?id&tags

	id := vangogh_local_data.ValueFromUrl(r.URL, "id")
	newTags := vangogh_local_data.ValuesFromUrl(r.URL, "tags")

	add, rem, err := vangogh_local_data.DiffLocalTags(id, newTags)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if len(add) > 0 {
		if err := vangogh_local_data.AddLocalTags([]string{id}, add, nil); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	if len(rem) > 0 {
		if err := vangogh_local_data.RemoveLocalTags([]string{id}, rem, nil); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	w.WriteHeader(http.StatusOK)
}
