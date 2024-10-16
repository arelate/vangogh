package rest

import (
	"github.com/arelate/vangogh/paths"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetLocalTagsApply(w http.ResponseWriter, r *http.Request) {

	// GET /local-tags/apply

	if err := r.ParseForm(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	var id string
	if len(r.Form["id"]) > 0 {
		id = r.Form["id"][0]
	}

	//don't skip if local-tags are empty as this might be a signal to remove existing tags
	newLocalTag := ""
	if len(r.Form["new-property-value"]) > 0 {
		newLocalTag = r.Form["new-property-value"][0]
	}

	localTags := r.Form["value"]
	if newLocalTag != "" {
		localTags = append(localTags, newLocalTag)
	}

	//TODO: Restore from vangogh
	//if err := patchLocalTag(http.DefaultClient, id, localTags); err != nil {
	//	http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
	//	return
	//}

	http.Redirect(w, r, paths.ProductId(id), http.StatusTemporaryRedirect)
}
