package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetInstallers(w http.ResponseWriter, r *http.Request) {

	// GET /installers?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok {
		switch pt {
		case vangogh_integration.PackProductType:
			// do nothing
		case vangogh_integration.DlcProductType:
			// do nothing
		case vangogh_integration.GameProductType:
			dls, err := getDownloadsList(id, operatingSystems, langCodes, noPatches)
			if err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
			gameInstallersPage := compton_pages.GameInstallers(id, dls, rdx)
			if err = gameInstallersPage.WriteResponse(w); err != nil {
				http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
				return
			}
		}
	}
}

func getDownloadsList(id string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noPatches bool) (vangogh_integration.DownloadsList, error) {

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return nil, err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	if !kvDetails.Has(id) {
		return nil, vangogh_integration.DetailsNotFoundErr(id)
	}

	// at this point we know that we should have product details in storage (see above)
	// so if we don't - that should be an error worth investigating
	det, err := vangogh_integration.UnmarshalDetails(id, kvDetails)
	if err != nil {
		return nil, err
	}

	if det == nil {
		return nil, vangogh_integration.NilDetailsErr(id)
	}

	dl, err := vangogh_integration.FromDetails(det, rdx)
	if err != nil {
		return nil, err
	}

	return dl.Only(operatingSystems,
		langCodes,
		[]vangogh_integration.DownloadType{vangogh_integration.AnyDownloadType},
		noPatches), nil
}
