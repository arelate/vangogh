package rest

import (
	"net/http"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
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

		add, rem, err := vangogh_integration.DiffTags(id, tags)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		acp, err := vangogh_integration.AbsCookiePath()
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		jar, err := coost.Read(gog_integration.DefaultUrl(), acp)
		if err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}

		hc := http.DefaultClient
		hc.Jar = jar

		if len(add) > 0 {
			if err := vangogh_integration.AddTags(hc, []string{id}, add, nil); err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
		}

		if len(rem) > 0 {
			if err := vangogh_integration.RemoveTags(hc, []string{id}, rem, nil); err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
		}
	}

	http.Redirect(w, r, "/product?id="+id, http.StatusTemporaryRedirect)
}
