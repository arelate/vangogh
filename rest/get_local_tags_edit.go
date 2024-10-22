package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetLocalTagsEdit(w http.ResponseWriter, r *http.Request) {

	// GET /local-tags/edit?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	selectedValues := make(map[string]any)
	if lt, ok := rdx.GetAllValues(vangogh_local_data.LocalTagsProperty, id); ok {
		for _, v := range lt {
			selectedValues[v] = nil
		}
	}

	localTags := make(map[string]string)
	for _, ltId := range rdx.Keys(vangogh_local_data.LocalTagsProperty) {
		if lts, ok := rdx.GetAllValues(vangogh_local_data.LocalTagsProperty, ltId); ok {
			for _, lt := range lts {
				localTags[lt] = lt
			}
		}
	}

	ltePage := compton_pages.TagsEditor(id, true, vangogh_local_data.LocalTagsProperty, localTags, selectedValues, rdx)
	if err := ltePage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}
