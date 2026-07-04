package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetGogChangelog(w http.ResponseWriter, r *http.Request) {

	// GET /gog-changelog/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)

	var pageTitle string
	if title, ok := rdx.GetLastVal(vangogh_integration.GogTitleProperty, id); ok {
		pageTitle = title
	}

	changelog, err := compton_data.GetKeyValuesBytes(id, vangogh_integration.GogChangelogKeyValues)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.GogChangelog(pageTitle, string(changelog))
	if err = p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}
