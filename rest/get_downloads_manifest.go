package rest

import (
	"encoding/json"
	"encoding/xml"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"net/http"
	"os"
	"path/filepath"
)

const applicationJsonContentType = "application/json"

func GetDownloadsManifest(w http.ResponseWriter, r *http.Request) {

	// GET /downloads-manifest?id

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

	pdm, err := getProductDownloadsManifest(id, dls, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if err = json.NewEncoder(w).Encode(pdm); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getProductDownloadsManifest(id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) (*vangogh_integration.DownloadsManifest, error) {
	dm := &vangogh_integration.DownloadsManifest{Id: id}
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		dm.Title = title
	}
	if slug, ok := rdx.GetLastVal(vangogh_integration.SlugProperty, id); ok {
		dm.Slug = slug
	}

	if image, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok {
		dm.Images.Image = image
	}
	if verticalImage, ok := rdx.GetLastVal(vangogh_integration.VerticalImageProperty, id); ok {
		dm.Images.VerticalImage = verticalImage
	}
	if hero, ok := rdx.GetLastVal(vangogh_integration.HeroProperty, id); ok {
		dm.Images.Hero = hero
	}
	if logo, ok := rdx.GetLastVal(vangogh_integration.LogoProperty, id); ok {
		dm.Images.Logo = logo
	}
	if icon, ok := rdx.GetLastVal(vangogh_integration.IconProperty, id); ok {
		dm.Images.Icon = icon
	}
	if iconSquare, ok := rdx.GetLastVal(vangogh_integration.IconSquareProperty, id); ok {
		dm.Images.IconSquare = iconSquare
	}
	if background, ok := rdx.GetLastVal(vangogh_integration.BackgroundProperty, id); ok {
		dm.Images.Background = background
	}

	for _, dl := range dls {
		link := vangogh_integration.ManifestDownloadLink{
			ManualUrl:      dl.ManualUrl,
			Name:           dl.Name,
			OS:             dl.OS.String(),
			Type:           dl.Type.String(),
			LanguageCode:   dl.LanguageCode,
			Version:        dl.Version,
			EstimatedBytes: dl.EstimatedBytes,
		}

		if dl.Type == vangogh_integration.DLC {
			link.Name = dl.ProductTitle
		}

		if relLocalDownloadPath, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl); ok {
			_, filename := filepath.Split(relLocalDownloadPath)
			link.LocalFilename = filename

			if md5, err := getMd5Checksum(relLocalDownloadPath); err == nil {
				link.Md5 = md5
			}
		}

		if muss, ok := rdx.GetLastVal(vangogh_integration.ManualUrlStatusProperty, dl.ManualUrl); ok && muss != "" {
			link.Status = muss
		} else {
			link.Status = vangogh_integration.ManualUrlStatusUnknown.String()
		}

		if vrs, ok := rdx.GetLastVal(vangogh_integration.ManualUrlValidationResultProperty, dl.ManualUrl); ok && vrs != "" {
			link.ValidationResult = vrs
		} else {
			link.ValidationResult = vangogh_integration.ValidationResultUnknown.String()
		}

		dm.DownloadLinks = append(dm.DownloadLinks, link)
	}

	return dm, nil
}

func getMd5Checksum(relLocalDownloadPath string) (string, error) {

	absChecksumPath, err := vangogh_integration.AbsLocalChecksumPath(relLocalDownloadPath)
	if err != nil {
		return "", err
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
