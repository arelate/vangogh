package rest

import (
	"github.com/arelate/vangogh/paths"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetTagsApply(w http.ResponseWriter, r *http.Request) {

	// GET /tags/apply

	if err := r.ParseForm(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	var id string
	if len(r.Form["id"]) > 0 {
		id = r.Form["id"][0]
	}

	owned := false
	if len(r.Form["condition"]) > 0 {
		owned = r.Form["condition"][0] == "true"
	}

	if owned {
		//don't skip if tags are empty as this might be a signal to remove existing tags
		tags := r.Form["value"]
		for i, t := range tags {
			tags[i] = t
		}

		//TODO: Restore from vangogh
		//if err := patchTag(http.DefaultClient, id, tags); err != nil {
		//	http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		//	return
		//}
	}

	http.Redirect(w, r, paths.ProductId(id), http.StatusTemporaryRedirect)
}
