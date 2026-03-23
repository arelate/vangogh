package rest

import (
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetDescription(w http.ResponseWriter, r *http.Request) {

	// GET /description?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	var pageTitle string
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		pageTitle = title
	}

	descOverview, err := compton_data.GetKeyValuesBytes(id, vangogh_integration.DescriptionOverviewKeyValues)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	descFeatures, err := compton_data.GetKeyValuesBytes(id, vangogh_integration.DescriptionFeaturesKeyValues)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.Description(id, pageTitle, string(descOverview), string(descFeatures), rdx)
	if err = p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
