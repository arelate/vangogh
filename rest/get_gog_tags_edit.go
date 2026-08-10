package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetGogTagsEdit(w http.ResponseWriter, r *http.Request) {

	// GET /gog-tags/edit?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get(vangogh_integration.UrlIdParameter)
	if !isAllowed(id, digits) {
		http.Error(w, errCharactersNotAllowed, http.StatusBadRequest)
		return
	}

	selectedValues := make(map[string]any)
	if tagIds, ok := rdx.GetAllValues(vangogh_integration.GogTagIdProperty, id); ok {
		for _, v := range tagIds {
			selectedValues[v] = nil
		}
	}

	tagNames := make(map[string]string)

	for k := range rdx.Keys(vangogh_integration.GogTagNameProperty) {
		if v, ok := rdx.GetAllValues(vangogh_integration.GogTagNameProperty, k); ok && len(v) > 0 {
			tagNames[k] = v[0]
		}
	}

	owned := false
	if op, ok := rdx.GetLastVal(vangogh_integration.GogOwnedProperty, id); ok && op == vangogh_integration.TrueValue {
		owned = true
	}

	ltePage := compton_pages.GogTagsEditor(id, owned, vangogh_integration.GogTagIdProperty, tagNames, selectedValues, rdx)
	if err := ltePage.Write(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
}
