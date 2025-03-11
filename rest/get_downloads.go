package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"net/http"
)

func GetInstallers(w http.ResponseWriter, r *http.Request) {

	// GET /installers?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	dls, err := getDownloads(id, operatingSystems, langCodes, noPatches, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.Downloads(id, dls, rdx)
	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}

func getDownloads(id string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noPatches bool,
	rdx redux.Readable) (vangogh_integration.DownloadsList, error) {

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return nil, err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	// if we don't have product details for the id this can mean two things:
	// - id represents a PACK that includes individual products (owned, not owned)
	// in this case we can iterate over included products and combine downloads for each one of them
	// - id represents a DLC that includes individual products (owned, not owned)
	// in this case we can iterate over required products and combine downloads for each one of them
	// - id represents a product that the user doesn't own
	// in this case we can remove basic product metadata (title, slug, etc) and no downloads

	if !kvDetails.Has(id) {
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok {
			switch pt {
			case "PACK":
				return relatedGamesDownloads(id, vangogh_integration.IncludesGamesProperty, operatingSystems, langCodes, noPatches, rdx)
			case "DLC":
				return relatedGamesDownloads(id, vangogh_integration.RequiresGamesProperty, operatingSystems, langCodes, noPatches, rdx)
			}
		}
		return nil, nil
	}

	// at this point we know that we should have product details in storage (see above)
	// so if we don't - that should be an error worth investigating
	det, err := vangogh_integration.UnmarshalDetails(id, kvDetails)
	if err != nil {
		return nil, err
	}

	if det == nil {
		return nil, nil
	}

	dl := make(vangogh_integration.DownloadsList, 0)

	if det != nil {
		dl, err = vangogh_integration.FromDetails(det, rdx)
		if err != nil {
			return nil, err
		}
	}

	return dl.Only(operatingSystems,
		langCodes,
		[]vangogh_integration.DownloadType{vangogh_integration.AnyDownloadType},
		noPatches), nil
}

func relatedGamesDownloads(id, property string,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noPatches bool,
	rdx redux.Readable) (vangogh_integration.DownloadsList, error) {
	if err := rdx.MustHave(property); err != nil {
		return nil, err
	}
	var relatedDownloadsList vangogh_integration.DownloadsList
	if relatedIds, ok := rdx.GetAllValues(property, id); ok {

		for _, relatedId := range relatedIds {
			if idl, err := getDownloads(relatedId, operatingSystems, langCodes, noPatches, rdx); err == nil {
				relatedDownloadsList = append(relatedDownloadsList, idl...)
			} else {
				return nil, err
			}
		}
		return relatedDownloadsList, nil
	}
	return relatedDownloadsList, nil
}
