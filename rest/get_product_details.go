package rest

import (
	"encoding/json/v2"
	"net/http"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

const applicationJsonContentType = "application/json"

func GetProductDetails(w http.ResponseWriter, r *http.Request) {

	// GET /api/product-details?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	q := r.URL.Query()

	id := q.Get(vangogh_integration.UrlIdParameter)

	det, err := getGogDetails(id)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	dls, err := getDownloadsList(det, operatingSystems, langCodes, noPatches)
	if err != nil && vangogh_integration.IsDetailsNotFound(err) {
		// details not found is only a fatal error for GAME products,
		// details don't exist for PACK and DLC products
		if productType, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, id); ok && productType == gog_integration.ProductTypeGame {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
			return
		}
	} else if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Add("Content-Type", applicationJsonContentType)

	pdm, err := getProductDetails(id, dls, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	if err = json.MarshalWrite(w, pdm); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}

func getProductDetails(id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) (*vangogh_integration.ProductDetails, error) {

	productDetails := &vangogh_integration.ProductDetails{Id: id}

	if slug, ok := rdx.GetLastVal(vangogh_integration.GogSlugProperty, id); ok {
		productDetails.Slug = slug
	}
	if steamAppId, ok := rdx.GetLastVal(vangogh_integration.GogSteamAppIdProperty, id); ok {
		productDetails.SteamAppId = steamAppId
	}
	if title, ok := rdx.GetLastVal(vangogh_integration.GogTitleProperty, id); ok {
		productDetails.Title = title
	}
	if productType, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, id); ok {
		productDetails.ProductType = productType
	}
	if oss, ok := rdx.GetAllValues(vangogh_integration.OperatingSystemsProperty, id); ok {
		oses := vangogh_integration.ParseManyOperatingSystems(oss)
		productDetails.OperatingSystems = oses
	}
	if developers, ok := rdx.GetAllValues(vangogh_integration.GogDevelopersProperty, id); ok {
		productDetails.Developers = developers
	}
	if publishers, ok := rdx.GetAllValues(vangogh_integration.GogPublishersProperty, id); ok {
		productDetails.Publishers = publishers
	}

	if image, ok := rdx.GetLastVal(vangogh_integration.GogImageProperty, id); ok {
		productDetails.Images.Image = image
	}
	if verticalImage, ok := rdx.GetLastVal(vangogh_integration.GogVerticalImageProperty, id); ok {
		productDetails.Images.VerticalImage = verticalImage
	}
	if hero, ok := rdx.GetLastVal(vangogh_integration.GogHeroProperty, id); ok {
		productDetails.Images.Hero = hero
	}
	if logo, ok := rdx.GetLastVal(vangogh_integration.GogLogoProperty, id); ok {
		productDetails.Images.Logo = logo
	}
	if icon, ok := rdx.GetLastVal(vangogh_integration.GogIconProperty, id); ok {
		productDetails.Images.Icon = icon
	}
	if iconSquare, ok := rdx.GetLastVal(vangogh_integration.GogIconSquareProperty, id); ok {
		productDetails.Images.IconSquare = iconSquare
	}
	if background, ok := rdx.GetLastVal(vangogh_integration.GogBackgroundProperty, id); ok {
		productDetails.Images.Background = background
	}

	for _, dl := range dls {

		dvs := vangogh_integration.NewManualUrlDvs(dl.ManualUrl, rdx)

		link := vangogh_integration.ProductDownloadLink{
			ManualUrl:        dl.ManualUrl,
			Name:             dl.Name,
			OperatingSystem:  dl.OS,
			DownloadType:     dl.DownloadType,
			LanguageCode:     dl.LanguageCode,
			Version:          dl.Version,
			EstimatedBytes:   dl.EstimatedBytes,
			DownloadStatus:   dvs.DownloadStatus(),
			ValidationStatus: dvs.ValidationStatus(),
		}

		if dl.DownloadType == vangogh_integration.DLC {
			link.Name = dl.ProductTitle
		}

		if filename, ok := rdx.GetLastVal(vangogh_integration.GogManualUrlFilenameProperty, dl.ManualUrl); ok {
			link.LocalFilename = filename
		}

		productDetails.DownloadLinks = append(productDetails.DownloadLinks, link)
	}

	switch productDetails.ProductType {
	case gog_integration.ProductTypePack:
		if includesGames, ok := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, id); ok {
			productDetails.IncludesGames = includesGames
		}
	case gog_integration.ProductTypeDlc:
		if requiresGames, ok := rdx.GetAllValues(vangogh_integration.GogRequiresGamesProperty, id); ok {
			productDetails.RequiresGames = requiresGames
		}
	case gog_integration.ProductTypeGame:
		fallthrough
	default:
		// do nothing
	}

	return productDetails, nil
}
