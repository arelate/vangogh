package rest

import (
	"encoding/json"
	"encoding/xml"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
	"os"
	"path/filepath"
	"strings"
)

const applicationJsonContentType = "application/json"

func GetDownloadsMetadata(w http.ResponseWriter, r *http.Request) {

	// GET /downloads-metadata?id&os&lang-code

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	id := q.Get("id")

	var oses []vangogh_local_data.OperatingSystem
	if q.Has(vangogh_local_data.OperatingSystemsProperty) {
		osesStr := strings.Split(q.Get(vangogh_local_data.OperatingSystemsProperty), ",")
		oses = vangogh_local_data.ParseManyOperatingSystems(osesStr)
	}
	if len(oses) == 0 {
		oses = operatingSystems
	}

	var langCodes []string
	if q.Has(vangogh_local_data.LanguageCodeProperty) {
		langCodes = strings.Split(q.Get(vangogh_local_data.LanguageCodeProperty), ",")
	}
	if len(langCodes) == 0 {
		langCodes = languageCodes
	}

	dls, err := getDownloads(id, oses, langCodes, noPatches, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	dm, err := getDownloadMetadata(id, dls, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if err := json.NewEncoder(w).Encode(dm); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getDownloadMetadata(id string, dls vangogh_local_data.DownloadsList, rdx kevlar.ReadableRedux) (*vangogh_local_data.DownloadMetadata, error) {
	dm := &vangogh_local_data.DownloadMetadata{Id: id}
	if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
		dm.Title = title
	}
	if slug, ok := rdx.GetLastVal(vangogh_local_data.SlugProperty, id); ok {
		dm.Slug = slug
	}

	for _, dl := range dls {
		link := vangogh_local_data.DownloadLink{
			ManualUrl:      dl.ManualUrl,
			Name:           dl.Name,
			OS:             dl.OS.String(),
			Type:           dl.Type.String(),
			LanguageCode:   dl.LanguageCode,
			Version:        dl.Version,
			EstimatedBytes: dl.EstimatedBytes,
		}

		if dl.Type == vangogh_local_data.DLC {
			link.Name = dl.ProductTitle
		}

		if relLocalDownloadPath, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, dl.ManualUrl); ok {
			_, filename := filepath.Split(relLocalDownloadPath)
			link.LocalFilename = filename

			if md5, err := getMd5Checksum(relLocalDownloadPath); err == nil {
				link.Md5 = md5
			}
		}

		dm.DownloadLinks = append(dm.DownloadLinks, link)
	}

	return dm, nil
}

func getMd5Checksum(relLocalDownloadPath string) (string, error) {

	absChecksumPath, err := vangogh_local_data.AbsLocalChecksumPath(relLocalDownloadPath)
	if err != nil {
		return "", err
	}

	chkFile, err := os.Open(absChecksumPath)
	if err != nil {
		return "", err
	}
	defer chkFile.Close()

	var chkData vangogh_local_data.ValidationFile
	if err := xml.NewDecoder(chkFile).Decode(&chkData); err != nil {
		return "", err
	}

	return chkData.MD5, nil
}
