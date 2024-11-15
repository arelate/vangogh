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
	languageCodes []string,
	excludePatches bool,
	rdx kevlar.ReadableRedux) (vangogh_local_data.DownloadsList, error) {

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return nil, err
	}

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
		[]vangogh_local_data.DownloadType{vangogh_local_data.AnyDownloadType},
		languageCodes,
		excludePatches), nil
}
