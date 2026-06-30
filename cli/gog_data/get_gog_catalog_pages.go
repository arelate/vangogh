package gog_data

import (
	"encoding/json/v2"
	"errors"
	"net/http"
	"slices"
	"strconv"
	"strings"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

var demoTitleSuffixes = []string{
	"demo", "prologue",
}

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
		vangogh_integration.GogCatalogPageProperties()...)
	if err != nil {
		return err
	}

	catalogPagesReductions := shared_data.InitReductions(vangogh_integration.GogCatalogPageProperties()...)

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

	if err = shared_data.WriteReductions(rdx, catalogPagesReductions); err != nil {
		return err
	}

	return reduceDemo(rdx)
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
			case vangogh_integration.GogTitleProperty:
				values = []string{cp.GetTitle()}
			case vangogh_integration.GogDevelopersProperty:
				values = cp.GetDevelopers()
			case vangogh_integration.GogPublishersProperty:
				values = cp.GetPublishers()
			case vangogh_integration.GogImageProperty:
				values = []string{gog_integration.ImageId(cp.GetImage())}
			case vangogh_integration.GogVerticalImageProperty:
				values = []string{gog_integration.ImageId(cp.GetVerticalImage())}
			case vangogh_integration.GogFeaturesProperty:
				values = cp.GetFeatures()
			case vangogh_integration.GogRatingProperty:
				values = []string{cp.GetRating()}
			case vangogh_integration.GogGenresProperty:
				values = cp.GetGenres()
			case vangogh_integration.GogOperatingSystemsProperty:
				values = cp.GetOperatingSystems()
			case vangogh_integration.GogSlugProperty:
				values = []string{cp.GetSlug()}
			case vangogh_integration.GogGlobalReleaseDateProperty:
				values = []string{cp.GetGlobalRelease()}
			case vangogh_integration.GogProductTypeProperty:
				values = []string{cp.GetProductType()}
			case vangogh_integration.GogStoreTagsProperty:
				values = cp.GetStoreTags()
			case vangogh_integration.GogBasePriceProperty:
				values = []string{cp.GetBasePrice()}
			case vangogh_integration.GogPriceProperty:
				values = []string{cp.GetPrice()}
			case vangogh_integration.GogIsFreeProperty:
				values = []string{strconv.FormatBool(cp.IsFree())}
			case vangogh_integration.GogIsModProperty:
				isMod := vangogh_integration.FalseValue
				for _, tag := range cp.Tags {
					if tag.Name == "Mod" {
						isMod = vangogh_integration.TrueValue
					}
				}
				values = []string{isMod}
			case vangogh_integration.GogIsDiscountedProperty:
				values = []string{strconv.FormatBool(cp.IsDiscounted())}
			case vangogh_integration.GogDiscountPercentageProperty:
				values = []string{strconv.Itoa(cp.GetDiscountPercentage())}
			case vangogh_integration.GogComingSoonProperty:
				values = []string{strconv.FormatBool(cp.GetComingSoon())}
			case vangogh_integration.GogPreOrderProperty:
				values = []string{strconv.FormatBool(cp.GetPreOrder())}
			case vangogh_integration.GogInDevelopmentProperty:
				values = []string{strconv.FormatBool(cp.GetInDevelopment())}
			case vangogh_integration.GogEditionsProperty:
				values = cp.GetEditions()
			case vangogh_integration.GogRootEditionsProperty:
				values = cp.GetRootEditions()
			case vangogh_integration.GogCatalogProductPageProperty:
				values = []string{page}
			case vangogh_integration.GogUserWishlistProperty:
				if !rdx.HasValue(vangogh_integration.GogUserWishlistProperty, cp.Id, vangogh_integration.TrueValue) {
					values = []string{vangogh_integration.FalseValue}
				}
			case vangogh_integration.GogScreenshotsProperty:
				values = gog_integration.ImageIds(cp.GetScreenshots()...)
			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][cp.Id] = values
			}
		}
	}

	return nil
}

func reduceDemo(rdx redux.Writeable) error {

	rda := nod.Begin(" reducing %s...", vangogh_integration.GogIsDemoProperty)
	defer rda.Done()

	if err := rdx.MustHave(
		vangogh_integration.GogTitleProperty,
		vangogh_integration.GogStoreTagsProperty,
		vangogh_integration.GogIsFreeProperty,
		vangogh_integration.GogIsDemoProperty); err != nil {
		return err
	}

	isDemo := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.GogTitleProperty) {

		if storeTags, ok := rdx.GetAllValues(vangogh_integration.GogStoreTagsProperty, id); ok {
			if slices.Contains(storeTags, "Demo") {
				isDemo[id] = []string{vangogh_integration.TrueValue}
				continue
			}
		}

		if isFree, sure := rdx.GetLastVal(vangogh_integration.GogIsFreeProperty, id); sure && isFree == vangogh_integration.TrueValue {
			if title, ok := rdx.GetLastVal(vangogh_integration.GogTitleProperty, id); ok {
				if titleParts := strings.Split(strings.ToLower(title), " "); len(titleParts) > 1 {

					lastTitlePart := titleParts[len(titleParts)-1]
					if slices.Contains(demoTitleSuffixes, lastTitlePart) {
						isDemo[id] = []string{vangogh_integration.TrueValue}
						continue
					}

					if len(titleParts) > 2 {
						lastTwoTitleParts := strings.Join([]string{titleParts[len(titleParts)-2], titleParts[len(titleParts)-1]}, " ")
						if slices.Contains(demoTitleSuffixes, lastTwoTitleParts) {
							isDemo[id] = []string{vangogh_integration.TrueValue}
							continue
						}
					}
				}
			}
		}

		isDemo[id] = []string{vangogh_integration.FalseValue}
	}

	return rdx.BatchReplaceValues(vangogh_integration.GogIsDemoProperty, isDemo)
}
