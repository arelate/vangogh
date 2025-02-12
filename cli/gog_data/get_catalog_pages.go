package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"strconv"
)

type propertyIdValues map[string]map[string][]string

func GetCatalogPages() error {

	gcpa := nod.NewProgress("getting %s...", vangogh_integration.CatalogPage)
	defer gcpa.Done()

	catalogPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.CatalogPage)
	if err != nil {
		return err
	}

	kvCatalogPages, err := kevlar.New(catalogPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = getGogPages(gog_integration.CatalogPageUrl, http.DefaultClient, kvCatalogPages, gcpa, true); err != nil {
		return err
	}

	return reduceCatalogPages(kvCatalogPages)
}

func reduceCatalogPages(kvCatalogPages kevlar.KeyValues) error {

	rcpa := nod.NewProgress(" reducing %s...", vangogh_integration.CatalogPage)
	defer rcpa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	catalogProductProperties := []string{
		vangogh_integration.TitleProperty,
		vangogh_integration.DevelopersProperty,
		vangogh_integration.PublishersProperty,
		vangogh_integration.ImageProperty,
		vangogh_integration.VerticalImageProperty,
		vangogh_integration.ScreenshotsProperty,
		vangogh_integration.GenresProperty,
		vangogh_integration.FeaturesProperty,
		vangogh_integration.RatingProperty,
		vangogh_integration.OperatingSystemsProperty,
		vangogh_integration.SlugProperty,
		vangogh_integration.GlobalReleaseDateProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.StoreTagsProperty,
		vangogh_integration.BasePriceProperty,
		vangogh_integration.PriceProperty,
		vangogh_integration.IsFreeProperty,
		vangogh_integration.IsDiscountedProperty,
		vangogh_integration.DiscountPercentageProperty,
		vangogh_integration.ComingSoonProperty,
		vangogh_integration.PreOrderProperty,
		vangogh_integration.InDevelopmentProperty,
	}

	rdx, err := redux.NewWriter(reduxDir, catalogProductProperties...)
	if err != nil {
		return err
	}

	rcpa.TotalInt(kvCatalogPages.Len())

	catalogPagesReductions := initReductions(catalogProductProperties...)

	for page := range kvCatalogPages.Keys() {
		if err = reduceCatalogPage(page, kvCatalogPages, catalogPagesReductions); err != nil {
			return err
		}

		rcpa.Increment()
	}

	return writeReductions(rdx, catalogPagesReductions)
}

func reduceCatalogPage(page string, kvCatalogPages kevlar.KeyValues, piv propertyIdValues) error {

	rcCatalogPage, err := kvCatalogPages.Get(page)
	if err != nil {
		return err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.NewDecoder(rcCatalogPage).Decode(&catalogPage); err != nil {
		return err
	}

	for _, cp := range catalogPage.Products {

		for property := range piv {
			var values []string

			switch property {
			case vangogh_integration.TitleProperty:
				values = []string{cp.GetTitle()}
			case vangogh_integration.DevelopersProperty:
				values = cp.GetDevelopers()
			case vangogh_integration.PublishersProperty:
				values = cp.GetPublishers()
			case vangogh_integration.ImageProperty:
				values = []string{cp.GetImage()}
			case vangogh_integration.VerticalImageProperty:
				values = []string{cp.GetVerticalImage()}
			case vangogh_integration.ScreenshotsProperty:
				values = cp.GetScreenshots()
			case vangogh_integration.FeaturesProperty:
				values = cp.GetFeatures()
			case vangogh_integration.RatingProperty:
				values = []string{cp.GetRating()}
			case vangogh_integration.GenresProperty:
				values = cp.GetGenres()
			case vangogh_integration.OperatingSystemsProperty:
				values = cp.GetOperatingSystems()
			case vangogh_integration.SlugProperty:
				values = []string{cp.GetSlug()}
			case vangogh_integration.GlobalReleaseDateProperty:
				values = []string{cp.GetGlobalRelease()}
			case vangogh_integration.ProductTypeProperty:
				values = []string{cp.GetProductType()}
			case vangogh_integration.StoreTagsProperty:
				values = cp.GetStoreTags()
			case vangogh_integration.BasePriceProperty:
				values = []string{cp.GetBasePrice()}
			case vangogh_integration.PriceProperty:
				values = []string{cp.GetPrice()}
			case vangogh_integration.IsFreeProperty:
				values = []string{strconv.FormatBool(cp.IsFree())}
			case vangogh_integration.IsDiscountedProperty:
				values = []string{strconv.FormatBool(cp.IsDiscounted())}
			case vangogh_integration.DiscountPercentageProperty:
				values = []string{strconv.Itoa(cp.GetDiscountPercentage())}
			case vangogh_integration.ComingSoonProperty:
				values = []string{strconv.FormatBool(cp.GetComingSoon())}
			case vangogh_integration.PreOrderProperty:
				values = []string{strconv.FormatBool(cp.GetPreOrder())}
			case vangogh_integration.InDevelopmentProperty:
				values = []string{strconv.FormatBool(cp.GetInDevelopment())}
			}

			piv[property][cp.Id] = values
		}
	}

	return nil
}
