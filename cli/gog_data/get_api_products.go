package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"net/http"
	"strconv"
)

func GetApiProducts(hc *http.Client, uat string, since int64, force bool) error {

	gapva := nod.NewProgress("getting %s...", vangogh_integration.ApiProductsV2)
	defer gapva.Done()

	if force {
		since = -1
	}

	catalogAccountProductIds, err := shared_data.GetCatalogAccountProducts(since)
	if err != nil {
		return err
	}

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProductsV2)
	if err != nil {
		return err
	}

	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gapva.TotalInt(len(catalogAccountProductIds))

	if err = fetch.Items(maps.Keys(catalogAccountProductIds), reqs.ApiProducts(hc, uat), kvApiProducts, gapva); err != nil {
		return err
	}

	return reduceApiProducts(kvApiProducts, since)
}

func reduceApiProducts(kvApiProducts kevlar.KeyValues, since int64) error {

	rapa := nod.Begin(" reducing %s...", vangogh_integration.ApiProductsV2)
	defer rapa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
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
		return err
	}

	apiProductReductions := shared_data.InitReductions(apiProductProperties...)

	updatedApiProducts := kvApiProducts.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedApiProducts {
		if err = reduceApiProduct(id, kvApiProducts, apiProductReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, apiProductReductions)
}

func reduceApiProduct(id string, kvApiProduct kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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

		piv[property][id] = values

	}

	return nil
}
