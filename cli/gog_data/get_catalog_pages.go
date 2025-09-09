package gog_data

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

const demoStoreTag = "Demo"

func GetCatalogPages(hc *http.Client, uat string, since int64, force bool) error {

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

	if err = fetchCatalogPages(reqs.CatalogPage(hc, uat), kvCatalogPages, gcpa); err != nil {
		return err
	}

	return ReduceCatalogPages(kvCatalogPages, since)
}

func ReduceCatalogPages(kvCatalogPages kevlar.KeyValues, since int64) error {

	pageType := vangogh_integration.CatalogPage

	rcpa := nod.Begin(" reducing %s...", pageType)
	defer rcpa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GOGCatalogPageProperties()...)
	if err != nil {
		return err
	}

	catalogPagesReductions := shared_data.InitReductions(vangogh_integration.GOGCatalogPageProperties()...)

	updatedCatalogPages := kvCatalogPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedCatalogPages {
		if !kvCatalogPages.Has(page) {
			nod.LogError(fmt.Errorf("%s is missing %s", pageType, page))
			continue
		}

		if err = reduceCatalogPage(page, kvCatalogPages, catalogPagesReductions, rdx); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, catalogPagesReductions)
}

func reduceCatalogPage(page string, kvCatalogPages kevlar.KeyValues, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

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
				values = []string{gog_integration.ImageId(cp.GetImage())}
			case vangogh_integration.VerticalImageProperty:
				values = []string{gog_integration.ImageId(cp.GetVerticalImage())}
			case vangogh_integration.ScreenshotsProperty:
				values = gog_integration.ImageIds(cp.GetScreenshots()...)
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
			case vangogh_integration.IsModProperty:
				isMod := vangogh_integration.FalseValue
				for _, tag := range cp.Tags {
					if tag.Name == "Mod" {
						isMod = vangogh_integration.TrueValue
					}
				}
				values = []string{isMod}
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
			case vangogh_integration.EditionsProperty:
				values = cp.GetEditions()
			case vangogh_integration.CatalogPageProductsProperty:
				values = []string{page}
			case vangogh_integration.UserWishlistProperty:
				if !rdx.HasValue(vangogh_integration.UserWishlistProperty, cp.Id, vangogh_integration.TrueValue) {
					values = []string{vangogh_integration.FalseValue}
				}
			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][cp.Id] = values
			}
		}
	}

	return nil
}
