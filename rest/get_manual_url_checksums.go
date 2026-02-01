package rest

import (
	"encoding/json"
	"encoding/xml"
	"net/http"
	"os"
	"path/filepath"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetManualUrlChecksums(w http.ResponseWriter, r *http.Request) {

	// GET /api/manual-url-checksums?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	id := q.Get("id")

	dls, err := getDownloadsList(id, operatingSystems, langCodes, noPatches)
	if err != nil && vangogh_integration.IsDetailsNotFound(err) {
		// details not found is only a fatal error for GAME products,
		// details don't exist for PACK and DLC products
		if productType, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok && productType == vangogh_integration.GameProductType {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	muc, err := getManualUrlChecksums(id, dls, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if err = json.NewEncoder(w).Encode(muc); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getManualUrlChecksums(id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) (map[string]string, error) {

	var slug string
	if sp, ok := rdx.GetLastVal(vangogh_integration.SlugProperty, id); ok {
		slug = sp
	}

	muChecksums := make(map[string]string, len(dls))

	for _, dl := range dls {

		if dl.DownloadType == vangogh_integration.Extra {
			continue
		}

		absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dl.DownloadType, downloadsLayout)
		if err != nil {
			return nil, err
		}

		if filename, ok := rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl); ok {
			absDownloadPath := filepath.Join(absSlugDownloadDir, filename)

			if md5, err := getMd5Checksum(absDownloadPath); err == nil {
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

	var chkData vangogh_integration.ValidationFile
	if err = xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return "", err
	}

	return chkData.MD5, nil
}
