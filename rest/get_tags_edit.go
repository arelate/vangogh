package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetTagsEdit(w http.ResponseWriter, r *http.Request) {

	// GET /tags/edit?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	selectedValues := make(map[string]any)
	if tagIds, ok := rdx.GetAllValues(vangogh_integration.TagIdProperty, id); ok {
		for _, v := range tagIds {
			selectedValues[v] = nil
		}
	}

	tagNames := make(map[string]string)

	for _, k := range rdx.Keys(vangogh_integration.TagNameProperty) {
		if v, ok := rdx.GetAllValues(vangogh_integration.TagNameProperty, k); ok && len(v) > 0 {
			tagNames[k] = v[0]
		}
	}

	owned := false
	if op, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && op == vangogh_integration.TrueValue {
		owned = true
	}

	ltePage := compton_pages.TagsEditor(id, owned, vangogh_integration.TagIdProperty, tagNames, selectedValues, rdx)
	if err := ltePage.Write(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}
