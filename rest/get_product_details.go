package rest

import (
	"encoding/json/v2"
	"net/http"

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

	if slug, ok := rdx.GetLastVal(vangogh_integration.SlugProperty, id); ok {
		productDetails.Slug = slug
	}
	if steamAppId, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, id); ok {
		productDetails.SteamAppId = steamAppId
	}
	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		productDetails.Title = title
	}
	if productType, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok {
		productDetails.ProductType = productType
	}
	if oss, ok := rdx.GetAllValues(vangogh_integration.OperatingSystemsProperty, id); ok {
		oses := vangogh_integration.ParseManyOperatingSystems(oss)
		productDetails.OperatingSystems = oses
	}
	if developers, ok := rdx.GetAllValues(vangogh_integration.DevelopersProperty, id); ok {
		productDetails.Developers = developers
	}
	if publishers, ok := rdx.GetAllValues(vangogh_integration.PublishersProperty, id); ok {
		productDetails.Publishers = publishers
	}

	if image, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok {
		productDetails.Images.Image = image
	}
	if verticalImage, ok := rdx.GetLastVal(vangogh_integration.VerticalImageProperty, id); ok {
		productDetails.Images.VerticalImage = verticalImage
	}
	if hero, ok := rdx.GetLastVal(vangogh_integration.HeroProperty, id); ok {
		productDetails.Images.Hero = hero
	}
	if logo, ok := rdx.GetLastVal(vangogh_integration.LogoProperty, id); ok {
		productDetails.Images.Logo = logo
	}
	if icon, ok := rdx.GetLastVal(vangogh_integration.IconProperty, id); ok {
		productDetails.Images.Icon = icon
	}
	if iconSquare, ok := rdx.GetLastVal(vangogh_integration.IconSquareProperty, id); ok {
		productDetails.Images.IconSquare = iconSquare
	}
	if background, ok := rdx.GetLastVal(vangogh_integration.BackgroundProperty, id); ok {
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

		if filename, ok := rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, dl.ManualUrl); ok {
			link.LocalFilename = filename
		}

		productDetails.DownloadLinks = append(productDetails.DownloadLinks, link)
	}

	switch productDetails.ProductType {
	case vangogh_integration.PackProductType:
		if includesGames, ok := rdx.GetAllValues(vangogh_integration.IncludesGamesProperty, id); ok {
			productDetails.IncludesGames = includesGames
		}
	case vangogh_integration.DlcProductType:
		if requiresGames, ok := rdx.GetAllValues(vangogh_integration.RequiresGamesProperty, id); ok {
			productDetails.RequiresGames = requiresGames
		}
	case vangogh_integration.GameProductType:
		fallthrough
	default:
		// do nothing
	}

	return productDetails, nil
}
