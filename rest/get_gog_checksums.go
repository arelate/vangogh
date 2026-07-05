package rest

import (
	"encoding/json/v2"
	"encoding/xml"
	"net/http"
	"os"
	"path/filepath"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogChecksums(w http.ResponseWriter, r *http.Request) {

	// GET /api/gog-checksums/{id}

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.PathValue(vangogh_integration.UrlIdParameter)

	muChecksums, err := getManualUrlChecksums(id, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", "application/json")

	if err = json.MarshalWrite(w, muChecksums); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getManualUrlChecksums(id string, rdx redux.Readable) (map[string]string, error) {

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

	var slug string
	if sp, ok := rdx.GetLastVal(vangogh_integration.GogSlugProperty, id); ok {
		slug = sp
	}

	muChecksums := make(map[string]string, len(dls))

	for _, dl := range dls {

		if dl.DownloadType == vangogh_integration.Extra {
			continue
		}

		var absSlugDownloadDir string
		absSlugDownloadDir, err = vangogh_integration.AbsSlugDownloadDir(slug, dl.DownloadType, downloadsLayout)
		if err != nil {
			return nil, err
		}

		if filename, ok := rdx.GetLastVal(vangogh_integration.GogManualUrlFilenameProperty, dl.ManualUrl); ok {
			absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

			var md5 string
			if md5, err = getMd5Checksum(absDownloadPath); err == nil {
				muChecksums[dl.ManualUrl] = md5
			} else {
				return nil, err
			}
		}
	}

	return muChecksums, nil
}

func getMd5Checksum(absDownloadPath string) (string, error) {

	absChecksumPath, err := vangogh_integration.AbsChecksumPath(absDownloadPath)
	if err != nil {
		return "", err
	}

	if _, err = os.Stat(absChecksumPath); os.IsNotExist(err) {
		return "", nil
	}

	chkFile, err := os.Open(absChecksumPath)
	if err != nil {
		return "", err
	}
	defer chkFile.Close()

	var chkData gog_integration.ValidationFile
	if err = xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return "", err
	}

	return chkData.MD5, nil
}
