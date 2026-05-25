package gog_data

import (
	"encoding/json/v2"
	"errors"
	"net/http"
	"strconv"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogCatalogPages(hc *http.Client, uat string, since int64) error {

	gcpa := nod.NewProgress("getting %s...", vangogh_integration.GogCatalogPage)
	defer gcpa.Done()

	gogCatalogPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogCatalogPage)
	if err != nil {
		return err
	}

	kvGogCatalogPages, err := kevlar.New(gogCatalogPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogCatalogPages(reqs.GogCatalogPage(hc, uat), kvGogCatalogPages, gcpa); err != nil {
		return err
	}

	return ReduceGogCatalogPages(kvGogCatalogPages, since)
}

func ReduceGogCatalogPages(kvGogCatalogPages kevlar.KeyValues, since int64) error {

	pageType := vangogh_integration.GogCatalogPage

	rcpa := nod.Begin(" reducing %s...", pageType)
	defer rcpa.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GOGCatalogPageProperties()...)
	if err != nil {
		return err
	}

	catalogPagesReductions := shared_data.InitReductions(vangogh_integration.GOGCatalogPageProperties()...)

	updatedCatalogPages := kvGogCatalogPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedCatalogPages {
		if !kvGogCatalogPages.Has(page) {
			nod.LogError(errors.New("missing: " + pageType.String() + ", " + page))
			continue
		}

		var catalogPage *gog_integration.CatalogPage
		if catalogPage, err = unmarshalGogCatalogPage(page, kvGogCatalogPages); err != nil {
			return err
		}

		if err = reduceGogCatalogPageProperties(page, catalogPage, catalogPagesReductions, rdx); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, catalogPagesReductions)
}

func unmarshalGogCatalogPage(page string, kvGogCatalogPages kevlar.KeyValues) (*gog_integration.CatalogPage, error) {
	rcGogCatalogPage, err := kvGogCatalogPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcGogCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.UnmarshalRead(rcGogCatalogPage, &catalogPage); err != nil {
		return nil, err
	}

	return &catalogPage, nil
}

func reduceGogCatalogPageProperties(page string, catalogPage *gog_integration.CatalogPage, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

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
			case vangogh_integration.RootEditionsProperty:
				values = cp.GetRootEditions()
			case vangogh_integration.CatalogPageProductsProperty:
				values = []string{page}
			case vangogh_integration.UserWishlistProperty:
				if !rdx.HasValue(vangogh_integration.UserWishlistProperty, cp.Id, vangogh_integration.TrueValue) {
					values = []string{vangogh_integration.FalseValue}
				}
			case vangogh_integration.ScreenshotsProperty:
				values = gog_integration.ImageIds(cp.GetScreenshots()...)
			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][cp.Id] = values
			}
		}
	}

	return nil
}
