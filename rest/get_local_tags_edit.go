package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetLocalTagsEdit(w http.ResponseWriter, r *http.Request) {

	// GET /local-tags/edit?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get(vangogh_integration.UrlIdParameter)

	selectedValues := make(map[string]any)
	if lt, ok := rdx.GetAllValues(vangogh_integration.VangoghLocalTagsProperty, id); ok {
		for _, v := range lt {
			selectedValues[v] = nil
		}
	}

	localTags := make(map[string]string)
	for ltId := range rdx.Keys(vangogh_integration.VangoghLocalTagsProperty) {
		if lts, ok := rdx.GetAllValues(vangogh_integration.VangoghLocalTagsProperty, ltId); ok {
			for _, lt := range lts {
				localTags[lt] = lt
			}
		}
	}

	ltePage := compton_pages.TagsEditor(id, true, vangogh_integration.VangoghLocalTagsProperty, localTags, selectedValues, rdx)
	if err := ltePage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}
