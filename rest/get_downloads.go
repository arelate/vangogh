package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetDownloads(w http.ResponseWriter, r *http.Request) {

	// GET /downloads?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	dls, err := getDownloads(id, operatingSystems, languageCodes, noPatches, rdx)
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
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	noPatches bool,
	rdx kevlar.ReadableRedux) (vangogh_local_data.DownloadsList, error) {

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return nil, err
	}

	hasDetails, err := vrDetails.Has(id)
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

	if !hasDetails {
		if pt, ok := rdx.GetLastVal(vangogh_local_data.ProductTypeProperty, id); ok {
			switch pt {
			case "PACK":
				return relatedGamesDownloads(id, vangogh_local_data.IncludesGamesProperty, operatingSystems, langCodes, noPatches, rdx)
			case "DLC":
				return relatedGamesDownloads(id, vangogh_local_data.RequiresGamesProperty, operatingSystems, langCodes, noPatches, rdx)
			}
		}
	}

	// at this point we know that we should have product details in storage (see above)
	// so if we don't - that should be an error worth investigating

	det, err := vrDetails.Details(id)
	if err != nil {
		return nil, err
	}

	dl := make(vangogh_local_data.DownloadsList, 0)

	if det != nil {
		dl, err = vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			return nil, err
		}
	}

	return dl.Only(operatingSystems,
		langCodes,
		[]vangogh_local_data.DownloadType{vangogh_local_data.AnyDownloadType},
		noPatches), nil
}

func relatedGamesDownloads(id, property string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	noPatches bool,
	rdx kevlar.ReadableRedux) (vangogh_local_data.DownloadsList, error) {
	if err := rdx.MustHave(property); err != nil {
		return nil, err
	}
	var relatedDownloadsList vangogh_local_data.DownloadsList
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
