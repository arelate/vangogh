package rest

import (
	"encoding/json/v2"
	"net/http"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogFilenames(w http.ResponseWriter, r *http.Request) {

	// GET /api/gog-filenames/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)

	muFilenames, err := getManualUrlFilenames(id, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", "application/json")

	if err = json.MarshalWrite(w, muFilenames); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getManualUrlFilenames(id string, rdx redux.Readable) (map[string]string, error) {

	det, err := getGogDetails(id)
	if err != nil {
		return nil, err
	}

	dls, err := getDownloadsList(det, operatingSystems, langCodes, noPatches)
	if err != nil && vangogh_integration.IsDetailsNotFound(err) {
		// details not found is only a fatal error for GAME products,
		// details don't exist for PACK and DLC products
		if productType, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, id); ok && productType == gog_integration.ProductTypeGame {
			return nil, err
		}
	} else if err != nil {
		return nil, err
	}

	muFilenames := make(map[string]string, len(dls))

	for _, dl := range dls {

		if dl.DownloadType == vangogh_integration.Extra {
			continue
		}

		if muFilename, ok := rdx.GetLastVal(vangogh_integration.GogManualUrlFilenameProperty, dl.ManualUrl); ok {
			muFilenames[dl.ManualUrl] = muFilename
		}
	}

	return muFilenames, nil
}
