package gog_data

import (
	"encoding/json/v2"
	"errors"
	"io"
	"maps"
	"net/http"
	"strconv"
	"strings"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogApiProducts(ids []string, hc *http.Client, uat string, since int64, force bool) error {

	gapva := nod.NewProgress("getting %s...", vangogh_integration.GogApiProducts)
	defer gapva.Done()

	var gogCatalogAccountProductIds map[string]any
	var err error

	if len(ids) > 0 {
		gogCatalogAccountProductIds = make(map[string]any)
		for _, id := range ids {
			gogCatalogAccountProductIds[id] = nil
		}
	} else {
		gogCatalogAccountProductIds, err = shared_data.GetGogCatalogAccountProducts(since)
		if err != nil {
			return err
		}
		gogCatalogAccountProductIds, err = shared_data.AppendGogEditions(gogCatalogAccountProductIds)
		if err != nil {
			return err
		}
	}

	gogApiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogApiProducts)
	if err != nil {
		return err
	}

	kvGogApiProducts, err := kevlar.New(gogApiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gapva.TotalInt(len(gogCatalogAccountProductIds))

	if err = fetch.Items(maps.Keys(gogCatalogAccountProductIds), reqs.GogApiProducts(hc, uat), kvGogApiProducts, gapva, force); err != nil {
		return err
	}

	return ReduceGogApiProducts(kvGogApiProducts, since, force)
}

func ReduceGogApiProducts(kvGogApiProducts kevlar.KeyValues, since int64, force bool) error {

	dataType := vangogh_integration.GogApiProducts

	rapa := nod.NewProgress(" reducing %s...", dataType)
	defer rapa.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogApiProductProperties()...)
	if err != nil {
		return err
	}

	apiProductReductions := shared_data.InitReductions(vangogh_integration.GogApiProductProperties()...)
	apiProductKeyValues, err := shared_data.InitKeyValues(vangogh_integration.GogApiProductsKeyValues()...)
	if err != nil {
		return err
	}

	updatedGogApiProducts := kvGogApiProducts.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedGogApiProducts {
		if !kvGogApiProducts.Has(id) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + id))
			continue
		}

		var ap *gog_integration.ApiProduct
		if ap, err = unmarshallGogApiProduct(id, kvGogApiProducts); err != nil {

		}

		if err = reduceGogApiProductProperties(id, ap, apiProductReductions); err != nil {
			return err
		}
		if err = reduceApiProductKeyValues(id, ap, apiProductKeyValues, force); err != nil {
			return err
		}
	}

	if err = shared_data.WriteReductions(rdx, apiProductReductions); err != nil {
		return err
	}

	return reduceOwned(rdx)
}

func unmarshallGogApiProduct(id string, kvGogApiProduct kevlar.KeyValues) (*gog_integration.ApiProduct, error) {
	rcGogApiProduct, err := kvGogApiProduct.Get(id)
	if err != nil {
		return nil, err
	}
	defer rcGogApiProduct.Close()

	var ap gog_integration.ApiProduct
	if err = json.UnmarshalRead(rcGogApiProduct, &ap); err != nil {
		return nil, err
	}

	return &ap, nil
}

func reduceGogApiProductProperties(id string, ap *gog_integration.ApiProduct, piv shared_data.PropertyIdValues) error {

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.GogTitleProperty:
			values = []string{ap.GetTitle()}
		case vangogh_integration.GogDevelopersProperty:
			values = ap.GetDevelopers()
		case vangogh_integration.GogPublishersProperty:
			values = ap.GetPublishers()
		case vangogh_integration.GogImageProperty:
			values = []string{gog_integration.ImageId(ap.GetImage())}
		case vangogh_integration.GogVerticalImageProperty:
			values = []string{gog_integration.ImageId(ap.GetVerticalImage())}
		case vangogh_integration.GogHeroProperty:
			values = []string{gog_integration.ImageId(ap.GetHero())}
		case vangogh_integration.GogLogoProperty:
			values = []string{gog_integration.ImageId(ap.GetLogo())}
		case vangogh_integration.GogIconProperty:
			values = []string{gog_integration.ImageId(ap.GetIcon())}
		case vangogh_integration.GogIconSquareProperty:
			values = []string{gog_integration.ImageId(ap.GetIconSquare())}
		case vangogh_integration.GogBackgroundProperty:
			values = []string{gog_integration.ImageId(ap.GetBackground())}
		case vangogh_integration.GogGenresProperty:
			values = ap.GetGenres()
		case vangogh_integration.GogFeaturesProperty:
			values = ap.GetFeatures()
		case vangogh_integration.GogSeriesProperty:
			values = []string{ap.GetSeries()}
		case vangogh_integration.GogYouTubeVideoIdProperty:
			values = ap.GetVideoIds()
		case vangogh_integration.GogOperatingSystemsProperty:
			values = ap.GetOperatingSystems()
		case vangogh_integration.GogIncludesGamesProperty:
			values = ap.GetIncludesGames()
		case vangogh_integration.GogIsIncludedByGamesProperty:
			values = ap.GetIsIncludedInGames()
		case vangogh_integration.GogRequiresGamesProperty:
			values = ap.GetRequiresGames()
		case vangogh_integration.GogIsRequiredByGamesProperty:
			values = ap.GetIsRequiredByGames()
		case vangogh_integration.GogModifiesGamesProperty:
			values = ap.GetModifiesGames()
		case vangogh_integration.GogIsModifiedByGamesProperty:
			values = ap.GetIsModifiedByGames()
		case vangogh_integration.GogIsModProperty:
			isMod := vangogh_integration.FalseValue
			for _, app := range ap.Embedded.Properties {
				if app.Name == "Mod" {
					isMod = vangogh_integration.TrueValue
				}
			}
			values = []string{isMod}
		case vangogh_integration.GogLanguageCodeProperty:
			values = ap.GetLanguageCodes()
		case vangogh_integration.GogGlobalReleaseDateProperty:
			values = []string{ap.GetGlobalRelease()}
		case vangogh_integration.GogReleaseDateProperty:
			values = []string{ap.GetGOGRelease()}
		case vangogh_integration.GogStoreUrlProperty:
			values = []string{ap.GetStoreUrl()}
		case vangogh_integration.GogForumUrlProperty:
			values = []string{ap.GetForumUrl()}
		case vangogh_integration.GogSupportUrlProperty:
			values = []string{ap.GetSupportUrl()}
		case vangogh_integration.GogProductTypeProperty:
			values = []string{ap.GetProductType()}
		case vangogh_integration.GogCopyrightsProperty:
			values = []string{ap.GetCopyrights()}
		case vangogh_integration.GogAdditionalRequirementsProperty:
			values = []string{ap.GetAdditionalRequirements()}
		case vangogh_integration.GogStoreTagsProperty:
			values = ap.GetStoreTags()
		case vangogh_integration.GogInDevelopmentProperty:
			values = []string{strconv.FormatBool(ap.GetInDevelopment())}
		case vangogh_integration.GogPreOrderProperty:
			values = []string{strconv.FormatBool(ap.GetPreOrder())}
		case vangogh_integration.GogScreenshotsProperty:
			values = gog_integration.ImageIds(ap.GetScreenshots()...)
		case vangogh_integration.GogOwnedProperty:
			continue
		case vangogh_integration.GogLicencesProperty:
			continue
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][id] = values
		}

	}

	return nil
}

func reduceApiProductKeyValues(id string, ap *gog_integration.ApiProduct, apiProductKeyValues map[string]kevlar.KeyValues, force bool) error {

	var err error
	var reader io.Reader

	for kv := range apiProductKeyValues {

		if apiProductKeyValues[kv].Has(id) && !force {
			continue
		}

		reader = nil

		switch kv {
		case vangogh_integration.GogDescriptionOverviewKeyValues:
			do := ap.GetDescriptionOverview()
			if do != "" {
				reader = strings.NewReader(do)
			}
		case vangogh_integration.GogDescriptionFeaturesKeyValues:
			df := ap.GetDescriptionFeatures()
			if df != "" {
				reader = strings.NewReader(df)
			}
		}

		if reader != nil {
			if err = apiProductKeyValues[kv].Set(id, reader); err != nil {
				return err
			}
		}
	}

	return nil
}

func reduceOwned(rdx redux.Writeable) error {

	roa := nod.Begin(" reducing %s...", vangogh_integration.GogOwnedProperty)
	defer roa.Done()

	owned := make(map[string][]string)

	// set all included products as owned
	for id := range rdx.Keys(vangogh_integration.GogLicencesProperty) {
		owned[id] = []string{vangogh_integration.TrueValue}
		if includesGames, ok := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, id); ok {
			for _, igId := range includesGames {
				owned[igId] = []string{vangogh_integration.TrueValue}
			}
		}
	}

	// set all PACKs as owned when all included products are owned
	for id := range rdx.Keys(vangogh_integration.GogIncludesGamesProperty) {
		if includesGames, ok := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, id); ok {
			includedGamesOwned := len(includesGames) > 0
			for _, igId := range includesGames {
				if !rdx.HasKey(vangogh_integration.GogLicencesProperty, igId) {
					includedGamesOwned = false
					break
				}
			}
			if includedGamesOwned {
				owned[id] = []string{vangogh_integration.TrueValue}
			}
		}
	}

	for id := range rdx.Keys(vangogh_integration.GogTitleProperty) {
		if _, ok := owned[id]; !ok {
			owned[id] = []string{vangogh_integration.FalseValue}
		}
	}

	// some products coming from licences will not have a title
	// we need to filter them out from owned, otherwise Search > Owned view
	// will have fewer products than declared (since product card are NOT
	// created for products without a title)
	for id := range owned {
		if !rdx.HasKey(vangogh_integration.GogTitleProperty, id) {
			owned[id] = []string{vangogh_integration.FalseValue}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.GogOwnedProperty, owned); err != nil {
		return err
	}

	return nil
}
