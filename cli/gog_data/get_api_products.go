package gog_data

import (
	"encoding/json"
	"fmt"
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

func GetApiProducts(ids []string, hc *http.Client, uat string, since int64, force bool) error {

	gapva := nod.NewProgress("getting %s...", vangogh_integration.ApiProducts)
	defer gapva.Done()

	var catalogAccountProductIds map[string]any
	var err error

	if len(ids) > 0 {
		catalogAccountProductIds = make(map[string]any)
		for _, id := range ids {
			catalogAccountProductIds[id] = nil
		}
	} else {
		catalogAccountProductIds, err = shared_data.GetCatalogAccountProducts(since)
		if err != nil {
			return err
		}
		catalogAccountProductIds, err = shared_data.AppendEditions(catalogAccountProductIds)
		if err != nil {
			return err
		}
	}

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProducts)
	if err != nil {
		return err
	}

	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gapva.TotalInt(len(catalogAccountProductIds))

	if err = fetch.Items(maps.Keys(catalogAccountProductIds), reqs.ApiProducts(hc, uat), kvApiProducts, gapva, force); err != nil {
		return err
	}

	return ReduceApiProducts(kvApiProducts, since)
}

func ReduceApiProducts(kvApiProducts kevlar.KeyValues, since int64) error {

	dataType := vangogh_integration.ApiProducts

	rapa := nod.Begin(" reducing %s...", dataType)
	defer rapa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GOGApiProductProperties()...)
	if err != nil {
		return err
	}

	apiProductReductions := shared_data.InitReductions(vangogh_integration.GOGApiProductProperties()...)

	updatedApiProducts := kvApiProducts.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedApiProducts {
		if !kvApiProducts.Has(id) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, id))
			continue
		}

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

	var ap gog_integration.ApiProduct
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
			values = []string{gog_integration.ImageId(ap.GetImage())}
		case vangogh_integration.VerticalImageProperty:
			values = []string{gog_integration.ImageId(ap.GetVerticalImage())}
		case vangogh_integration.HeroProperty:
			values = []string{gog_integration.ImageId(ap.GetHero())}
		case vangogh_integration.LogoProperty:
			values = []string{gog_integration.ImageId(ap.GetLogo())}
		case vangogh_integration.IconProperty:
			values = []string{gog_integration.ImageId(ap.GetIcon())}
		case vangogh_integration.IconSquareProperty:
			values = []string{gog_integration.ImageId(ap.GetIconSquare())}
		case vangogh_integration.BackgroundProperty:
			values = []string{gog_integration.ImageId(ap.GetBackground())}
		case vangogh_integration.ScreenshotsProperty:
			values = gog_integration.ImageIds(ap.GetScreenshots()...)
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
		case vangogh_integration.ModifiesGamesProperty:
			values = ap.GetModifiesGames()
		case vangogh_integration.IsModifiedByGamesProperty:
			values = ap.GetIsModifiedByGames()
		case vangogh_integration.IsModProperty:
			isMod := vangogh_integration.FalseValue
			for _, app := range ap.Embedded.Properties {
				if app.Name == "Mod" {
					isMod = vangogh_integration.TrueValue
				}
			}
			values = []string{isMod}
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

		if shared_data.IsNotEmpty(values...) {
			piv[property][id] = values
		}

	}

	return nil
}
