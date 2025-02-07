package gog_data

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"strconv"
)

func GetApiProducts(since int64, force bool) error {

	gapva := nod.NewProgress("getting %s...", vangogh_integration.ApiProductsV2)
	defer gapva.EndWithResult("done")

	if force {
		since = -1
	}

	catalogAccountProductIds, err := getCatalogAccountProducts(since)
	if err != nil {
		return gapva.EndWithError(err)
	}

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProductsV2)
	if err != nil {
		return gapva.EndWithError(err)
	}
	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return gapva.EndWithError(err)
	}

	ids := make([]string, 0, len(catalogAccountProductIds))
	for id := range catalogAccountProductIds {
		ids = append(ids, id)
	}

	// TODO: Save errors and dates and don't request them again in 30 days
	if itemErrs := getGogItems(gog_integration.ApiProductV2Url, http.DefaultClient, kvApiProducts, gapva, ids...); len(itemErrs) > 0 {
		return fmt.Errorf("get %s errors: %v", vangogh_integration.ApiProductsV2, itemErrs)
	}

	updatedApiProducts := kvApiProducts.Since(since, kevlar.Create, kevlar.Update)

	uapIds := make([]string, 0)
	for id := range updatedApiProducts {
		uapIds = append(uapIds, id)
	}

	return reduceApiProducts(kvApiProducts, uapIds...)
}

func getCatalogAccountProducts(since int64) (map[string]any, error) {
	catalogAccountProductIds := make(map[string]any)

	catalogProductIds, err := getCatalogPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for _, cpId := range catalogProductIds {
		catalogAccountProductIds[cpId] = nil
	}

	accountProductIds, err := getAccountPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for _, apId := range accountProductIds {
		catalogAccountProductIds[apId] = nil
	}

	return catalogAccountProductIds, nil
}

func getCatalogPagesProducts(since int64) ([]string, error) {

	gcppa := nod.NewProgress(" enumerating %s...", vangogh_integration.CatalogPage)
	defer gcppa.EndWithResult("done")

	catalogPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.CatalogPage)
	if err != nil {
		return nil, err
	}
	kvCatalogPages, err := kevlar.New(catalogPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	updatedPages := make([]string, 0)
	for page := range kvCatalogPages.Since(since, kevlar.Create, kevlar.Update) {
		updatedPages = append(updatedPages, page)
	}

	gcppa.TotalInt(len(updatedPages))

	catalogProductIds := make([]string, 0, kvCatalogPages.Len())

	for _, page := range updatedPages {
		ids, err := getCatalogPageProducts(page, kvCatalogPages)
		if err != nil {
			return nil, err
		}
		catalogProductIds = append(catalogProductIds, ids...)

		gcppa.Increment()
	}

	return catalogProductIds, nil
}

func getCatalogPageProducts(page string, kvCatalogPages kevlar.KeyValues) ([]string, error) {
	rcCatalogPage, err := kvCatalogPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.NewDecoder(rcCatalogPage).Decode(&catalogPage); err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(catalogPage.Products))

	for _, catalogProduct := range catalogPage.Products {
		ids = append(ids, catalogProduct.Id)
	}

	return ids, nil
}

func getAccountPagesProducts(since int64) ([]string, error) {

	gappa := nod.NewProgress(" enumerating %s...", vangogh_integration.AccountPage)
	defer gappa.EndWithResult("done")

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return nil, err
	}
	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	updatedPages := make([]string, 0)
	for page := range kvAccountPages.Since(since, kevlar.Create, kevlar.Update) {
		updatedPages = append(updatedPages, page)
	}

	gappa.TotalInt(len(updatedPages))

	accountProductIds := make([]string, 0, kvAccountPages.Len())

	for _, page := range updatedPages {
		ids, err := getAccountPageProducts(page, kvAccountPages)
		if err != nil {
			return nil, err
		}
		accountProductIds = append(accountProductIds, ids...)

		gappa.Increment()
	}

	return accountProductIds, nil
}

func getAccountPageProducts(page string, kvAccountPages kevlar.KeyValues) ([]string, error) {
	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.NewDecoder(rcAccountPage).Decode(&accountPage); err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(accountPage.Products))

	for _, accountProduct := range accountPage.Products {
		ids = append(ids, strconv.Itoa(accountProduct.Id))
	}

	return ids, nil
}

func reduceApiProducts(kvApiProducts kevlar.KeyValues, ids ...string) error {

	rapa := nod.NewProgress(" reducing %s...", vangogh_integration.ApiProductsV2)
	defer rapa.EndWithResult("done")

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return rapa.EndWithError(err)
	}

	apiProductProperties := []string{
		vangogh_integration.TitleProperty,
		vangogh_integration.DevelopersProperty,
		vangogh_integration.PublishersProperty,
		vangogh_integration.LanguageCodeProperty,
		vangogh_integration.ImageProperty,
		vangogh_integration.VerticalImageProperty,
		vangogh_integration.HeroProperty,
		vangogh_integration.LogoProperty,
		vangogh_integration.IconProperty,
		vangogh_integration.IconSquareProperty,
		vangogh_integration.BackgroundProperty,
		vangogh_integration.ScreenshotsProperty,
		vangogh_integration.GenresProperty,
		vangogh_integration.FeaturesProperty,
		vangogh_integration.SeriesProperty,
		vangogh_integration.VideoIdProperty,
		vangogh_integration.OperatingSystemsProperty,
		vangogh_integration.RequiresGamesProperty,
		vangogh_integration.IsRequiredByGamesProperty,
		vangogh_integration.IncludesGamesProperty,
		vangogh_integration.IsIncludedByGamesProperty,
		vangogh_integration.GlobalReleaseDateProperty,
		vangogh_integration.GOGReleaseDateProperty,
		vangogh_integration.StoreUrlProperty,
		vangogh_integration.ForumUrlProperty,
		vangogh_integration.SupportUrlProperty,
		vangogh_integration.DescriptionOverviewProperty,
		vangogh_integration.DescriptionFeaturesProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.CopyrightsProperty,
		vangogh_integration.StoreTagsProperty,
		vangogh_integration.AdditionalRequirementsProperty,
		vangogh_integration.InDevelopmentProperty,
		vangogh_integration.PreOrderProperty,
	}

	rdx, err := redux.NewWriter(reduxDir, apiProductProperties...)
	if err != nil {
		return rapa.EndWithError(err)
	}

	rapa.TotalInt(len(ids))

	apiProductReductions := initReductions(apiProductProperties...)

	for _, id := range ids {
		if err = reduceApiProduct(id, kvApiProducts, apiProductReductions); err != nil {
			return rapa.EndWithError(err)
		}

		rapa.Increment()
	}

	return writeReductions(rdx, apiProductReductions)
}

func reduceApiProduct(id string, kvApiProduct kevlar.KeyValues, piv propertyIdValues) error {

	rcApiProduct, err := kvApiProduct.Get(id)
	if err != nil {
		return err
	}
	defer rcApiProduct.Close()

	var ap gog_integration.ApiProductV2
	if err = json.NewDecoder(rcApiProduct).Decode(&ap); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.TitleProperty:
			values = []string{ap.GetTitle()}
		case vangogh_integration.DevelopersProperty:
			values = ap.GetDevelopers()
		case vangogh_integration.PublishersProperty:
			values = ap.GetPublishers()
		case vangogh_integration.ImageProperty:
			values = []string{ap.GetImage()}
		case vangogh_integration.VerticalImageProperty:
			values = []string{ap.GetVerticalImage()}
		case vangogh_integration.HeroProperty:
			values = []string{ap.GetHero()}
		case vangogh_integration.LogoProperty:
			values = []string{ap.GetLogo()}
		case vangogh_integration.IconProperty:
			values = []string{ap.GetIcon()}
		case vangogh_integration.IconSquareProperty:
			values = []string{ap.GetIconSquare()}
		case vangogh_integration.BackgroundProperty:
			values = []string{ap.GetBackground()}
		case vangogh_integration.ScreenshotsProperty:
			values = ap.GetScreenshots()
		case vangogh_integration.GenresProperty:
			values = ap.GetGenres()
		case vangogh_integration.FeaturesProperty:
			values = ap.GetFeatures()
		case vangogh_integration.SeriesProperty:
			values = []string{ap.GetSeries()}
		case vangogh_integration.VideoIdProperty:
			values = ap.GetVideoIds()
		case vangogh_integration.OperatingSystemsProperty:
			values = ap.GetOperatingSystems()
		case vangogh_integration.IncludesGamesProperty:
			values = ap.GetIncludesGames()
		case vangogh_integration.IsIncludedByGamesProperty:
			values = ap.GetIsIncludedInGames()
		case vangogh_integration.RequiresGamesProperty:
			values = ap.GetRequiresGames()
		case vangogh_integration.IsRequiredByGamesProperty:
			values = ap.GetIsRequiredByGames()
		case vangogh_integration.LanguageCodeProperty:
			values = ap.GetLanguageCodes()
		case vangogh_integration.GlobalReleaseDateProperty:
			values = []string{ap.GetGlobalRelease()}
		case vangogh_integration.GOGReleaseDateProperty:
			values = []string{ap.GetGOGRelease()}
		case vangogh_integration.StoreUrlProperty:
			values = []string{ap.GetStoreUrl()}
		case vangogh_integration.ForumUrlProperty:
			values = []string{ap.GetForumUrl()}
		case vangogh_integration.SupportUrlProperty:
			values = []string{ap.GetSupportUrl()}
		case vangogh_integration.DescriptionOverviewProperty:
			values = []string{ap.GetDescriptionOverview()}
		case vangogh_integration.DescriptionFeaturesProperty:
			values = []string{ap.GetDescriptionFeatures()}
		case vangogh_integration.ProductTypeProperty:
			values = []string{ap.GetProductType()}
		case vangogh_integration.CopyrightsProperty:
			values = []string{ap.GetCopyrights()}
		case vangogh_integration.AdditionalRequirementsProperty:
			values = []string{ap.GetAdditionalRequirements()}
		case vangogh_integration.StoreTagsProperty:
			values = ap.GetStoreTags()
		case vangogh_integration.InDevelopmentProperty:
			values = []string{strconv.FormatBool(ap.GetInDevelopment())}
		case vangogh_integration.PreOrderProperty:
			values = []string{strconv.FormatBool(ap.GetPreOrder())}
		}

		piv[property][strconv.Itoa(ap.GetId())] = values

	}

	return nil
}
