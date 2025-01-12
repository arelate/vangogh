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
)

const applicationJsonContentType = "application/json"

func GetMetadata(w http.ResponseWriter, r *http.Request) {

	// GET /metadata?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	id := q.Get("id")

	dls, err := getDownloads(id, operatingSystems, langCodes, noPatches, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	dm, err := getProductMetadata(id, dls, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if err := json.NewEncoder(w).Encode(dm); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getProductMetadata(id string, dls vangogh_local_data.DownloadsList, rdx kevlar.ReadableRedux) (*vangogh_local_data.TheoMetadata, error) {
	tm := &vangogh_local_data.TheoMetadata{Id: id}
	if title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id); ok {
		tm.Title = title
	}
	if slug, ok := rdx.GetLastVal(vangogh_local_data.SlugProperty, id); ok {
		tm.Slug = slug
	}

	if image, ok := rdx.GetLastVal(vangogh_local_data.ImageProperty, id); ok {
		tm.Images.Image = image
	}
	if verticalImage, ok := rdx.GetLastVal(vangogh_local_data.VerticalImageProperty, id); ok {
		tm.Images.VerticalImage = verticalImage
	}
	if hero, ok := rdx.GetLastVal(vangogh_local_data.HeroProperty, id); ok {
		tm.Images.Hero = hero
	}
	if logo, ok := rdx.GetLastVal(vangogh_local_data.LogoProperty, id); ok {
		tm.Images.Logo = logo
	}
	if icon, ok := rdx.GetLastVal(vangogh_local_data.IconProperty, id); ok {
		tm.Images.Icon = icon
	}
	if iconSquare, ok := rdx.GetLastVal(vangogh_local_data.IconSquareProperty, id); ok {
		tm.Images.IconSquare = iconSquare
	}

	for _, dl := range dls {
		link := vangogh_local_data.TheoDownloadLink{
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

		if muss, ok := rdx.GetLastVal(vangogh_local_data.ManualUrlStatusProperty, dl.ManualUrl); ok && muss != "" {
			link.Status = muss
		} else {
			link.Status = vangogh_local_data.ManualUrlStatusUnknown.String()
		}

		if vrs, ok := rdx.GetLastVal(vangogh_local_data.ManualUrlValidationResultProperty, dl.ManualUrl); ok && vrs != "" {
			link.ValidationResult = vrs
		} else {
			link.ValidationResult = vangogh_local_data.ValidationResultUnknown.String()
		}

		tm.DownloadLinks = append(tm.DownloadLinks, link)
	}

	return tm, nil
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
