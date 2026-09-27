package rest

import (
	"net/http"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

func GetGogInstallers(w http.ResponseWriter, r *http.Request) {

	// GET /gog/installers/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)

	if gogIsAccountProduct, ok := rdx.GetLastVal(vangogh_integration.GogIsAccountProductProperty, id); !ok || gogIsAccountProduct != vangogh_integration.TrueValue {
		w.WriteHeader(http.StatusNoContent)
		return
	}

	det, err := getGogDetails(id)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	dls, err := getDownloadsList(det, operatingSystems, langCodes, noPatches)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}
	gameInstallersPage := compton_pages.GogInstallers(id, det.Messages, dls, rdx)
	if err = gameInstallersPage.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

}

func getGogDetails(id string) (*gog_integration.Details, error) {

	gogDetailsDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogDetails)

	kvGogDetails, err := kevlar.New(gogDetailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	if !kvGogDetails.Has(id) {
		return nil, vangogh_integration.DetailsNotFoundErr(id)
	}

	// at this point we know that we should have product details in storage (see above)
	// so if we don't - that should be an error worth investigating
	det, err := vangogh_integration.UnmarshalDetails(id, kvGogDetails)
	if err != nil {
		return nil, err
	}

	if det == nil {
		return nil, vangogh_integration.NilDetailsErr(id)
	}

	return det, nil
}

func getDownloadsList(det *gog_integration.Details,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noPatches bool) (vangogh_integration.DownloadsList, error) {

	dl, err := vangogh_integration.FromDetails(det)
	if err != nil {
		return nil, err
	}

	return dl.Only(operatingSystems,
		langCodes,
		false,
		false,
		noPatches), nil
}
