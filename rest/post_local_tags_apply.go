package rest

import (
	"net/http"
	"path"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func PostLocalTagsApply(w http.ResponseWriter, r *http.Request) {

	// POST /local-tags/apply/{id}

	if err := r.ParseForm(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)
	if !isAllowed(id, digits) {
		http.Error(w, errCharactersNotAllowed, http.StatusBadRequest)
		return
	}

	//don't skip if local-tags are empty as this might be a signal to remove existing tags
	newLocalTag := ""
	if len(r.Form[vangogh_integration.UrlNewValueParameter]) > 0 {
		newLocalTag = r.Form[vangogh_integration.UrlNewValueParameter][0]
	}

	localTags := r.Form[vangogh_integration.UrlValueParameter]
	if newLocalTag != "" {
		localTags = append(localTags, newLocalTag)
	}

	add, rem, err := vangogh_integration.DiffLocalTags(id, localTags)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if len(add) > 0 {
		if err = vangogh_integration.AddLocalTags([]string{id}, add, nil); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	if len(rem) > 0 {
		if err = vangogh_integration.RemoveLocalTags([]string{id}, rem, nil); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	}

	w.Header().Set("Location", path.Join("/gog/product", id))
	w.WriteHeader(http.StatusSeeOther)
}
