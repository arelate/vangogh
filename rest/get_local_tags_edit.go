package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetLocalTagsEdit(w http.ResponseWriter, r *http.Request) {

	// GET /local-tags/edit?id

	id := r.URL.Query().Get("id")

	selectedValues := make(map[string]any)
	if lt, ok := rdx.GetAllValues(vangogh_local_data.LocalTagsProperty, id); ok {
		for _, v := range lt {
			selectedValues[v] = nil
		}
	}

	// TODO: Restore
	//localTags := make(map[string]string)
	//for _, v := range localTagsDigest[vangogh_local_data.LocalTagsProperty] {
	//	localTags[v] = v
	//}

	ltePage := compton_pages.TagsEditor(id, true, vangogh_local_data.LocalTagsProperty, nil, selectedValues, rdx)
	if err := ltePage.WriteContent(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}
