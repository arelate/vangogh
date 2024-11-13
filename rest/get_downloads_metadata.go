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

type downloadMetadata struct {
	Id            string         `json:"id"`
	Slug          string         `json:"slug"`
	Title         string         `json:"title"`
	DownloadLinks []downloadLink `json:"download-links,omitempty"`
}

type downloadLink struct {
	ManualUrl      string `json:"manual-url"`
	LocalFilename  string `json:"local-filename"`
	Md5            string `json:"md5"`
	OS             string `json:"os"`
	Type           string `json:"type"`
	LanguageCode   string `json:"language-code"`
	Version        string `json:"version"`
	EstimatedBytes int    `json:"estimated-bytes"`
}

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

	dls, err := getDownloads(id, oses, langCodes, excludePatches, rdx)
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

func getDownloadMetadata(id string, dls vangogh_local_data.DownloadsList, rdx kevlar.ReadableRedux) (*downloadMetadata, error) {
	dm := &downloadMetadata{Id: id}
	if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
		dm.Title = title
	}
	if slug, ok := rdx.GetLastVal(vangogh_local_data.SlugProperty, id); ok {
		dm.Slug = slug
	}

	for _, download := range dls {
		link := downloadLink{
			ManualUrl:      download.ManualUrl,
			OS:             download.OS.String(),
			Type:           download.Type.String(),
			LanguageCode:   download.LanguageCode,
			Version:        download.Version,
			EstimatedBytes: download.EstimatedBytes,
		}

		if relLocalDownloadPath, ok := rdx.GetLastVal(vangogh_local_data.LocalManualUrlProperty, download.ManualUrl); ok {
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
