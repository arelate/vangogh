package rest

import (
	"net/http"
	"path"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
)

func PostGogTagsApply(w http.ResponseWriter, r *http.Request) {

	// POST /gog/tags/apply/{id}

	if err := r.ParseForm(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)
	if !isAllowed(id, digits) {
		http.Error(w, errCharactersNotAllowed, http.StatusBadRequest)
		return
	}

	owned := false
	if len(r.Form[vangogh_integration.UrlConditionParameter]) > 0 {
		owned = r.Form[vangogh_integration.UrlConditionParameter][0] == "true"
	}

	if owned {
		//don't skip if tags are empty as this might be a signal to remove existing tags
		tags := r.Form[vangogh_integration.UrlValueParameter]
		for i, t := range tags {
			tags[i] = t
		}

		add, rem, err := vangogh_integration.DiffTags(id, tags)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		acp := vangogh_integration.AbsCookiesPath()
		jar, err := coost.Read(gog_integration.HostUrl(), acp)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		hc := http.DefaultClient
		hc.Jar = jar

		if len(add) > 0 {
			if err = vangogh_integration.AddTags(hc, []string{id}, add, nil); err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
		}

		if len(rem) > 0 {
			if err = vangogh_integration.RemoveTags(hc, []string{id}, rem, nil); err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
		}
	}

	w.Header().Set("Location", path.Join("/gog/product", id))
	w.WriteHeader(http.StatusSeeOther)
}
