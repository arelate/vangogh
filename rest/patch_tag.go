package rest

import (
	"github.com/arelate/gog_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
	"net/http"
)

func PatchTag(w http.ResponseWriter, r *http.Request) {

	// PATCH /tag?id&tags

	id := vangogh_local_data.ValueFromUrl(r.URL, "id")
	newTags := vangogh_local_data.ValuesFromUrl(r.URL, "tags")

	add, rem, err := vangogh_local_data.DiffTags(id, newTags)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	hc, err := coost.NewHttpClientFromFile(vangogh_local_data.AbsCookiePath(), gog_integration.GogHost)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if len(add) > 0 {
		if err := vangogh_local_data.AddTags(hc, []string{id}, add, nil); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	if len(rem) > 0 {
		if err := vangogh_local_data.RemoveTags(hc, []string{id}, rem, nil); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	w.WriteHeader(http.StatusOK)
}
