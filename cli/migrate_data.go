package cli

import (
	"io"
	"net/url"
	"os"
	"path/filepath"
	"slices"
	"strconv"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

// Data scheme version log:
// 1. local-manual-url -> manual-url-filename
// 2. rm -rf metadata/_type_errors
// 3. rename updates sections (remove old section titles) // v1.1.8
// 4. rename _binaries to _wine-binaries
// 5. deprecate aggregated-rating property
// 6. rename logs from year-month-date-hour-minute-second.log to sync-year-month-date-hour-minute-second.log // v1.1.9
// 7. remove previously cascaded validations // v1.2.1
// 8. fix incorrect dlc, extras sub-directories in downloads // 1.2.6
// 9. (internal bump)
// 10. remove dehydrated-image, rep-color
// 11. remove description-overview, description-features, changelog properties // 1.2.7
// 12. rename GOG.com product type directories
// 13. reset GetDataErrorDateProperty, GetDataErrorMessageProperty, GetDataLastUpdatedProperty since the format of the key has changed
// 14. remove user-access-token value from gog-user-access-token KeyValues
// 15. rename properties to gog-properties

const (
	latestDataSchema = 16
)

func MigrateDataHandler(u *url.URL) error {

	q := u.Query()

	force := q.Has(vangogh_integration.UrlForceParameter)

	return MigrateData(force)
}

func MigrateData(force bool) error {
	mda := nod.NewProgress("migrating data to the latest schema...")
	defer mda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.DataSchemeVersionProperty)
	if err != nil {
		return err
	}

	// debug helper - uncomment to re-run latest migration
	//if err = rdx.ReplaceValues(vangogh_integration.DataSchemeVersionProperty, vangogh_integration.DataSchemeVersionProperty, strconv.Itoa(latestDataSchema-1)); err != nil {
	//	return err
	//}

	var currentDataSchema int
	if !force {
		currentDataSchema, err = getCurrentDataSchema(rdx)
		if err != nil {
			return err
		}
	}

	if currentDataSchema == latestDataSchema {
		mda.EndWithResult("already at the latest data schema")
		return nil
	}

	mda.TotalInt(latestDataSchema - currentDataSchema)

	for schema := currentDataSchema; schema < latestDataSchema; schema++ {
		switch schema {
		case 9:
			// internal version
		case 10:
			if err = removeDehydratedImageRepColor(); err != nil {
				return err
			}
		case 11:
			if err = removeLargeTextProperties(); err != nil {
				return err
			}
		case 12:
			if err = renameGogProductTypeDirectories(); err != nil {
				return err
			}
		case 13:
			if err = resetGetDataProperties(); err != nil {
				return err
			}
		case 14:
			if err = removeUserAccessTokenValue(); err != nil {
				return err
			}
		case 15:
			if err = renameGogProperties(); err != nil {
				return err
			}
		}

		mda.Increment()
	}

	rdx, err = redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.DataSchemeVersionProperty)
	if err != nil {
		return err
	}

	return setLatestDataSchema(rdx)
}

func getCurrentDataSchema(rdx redux.Readable) (int, error) {

	if err := rdx.MustHave(vangogh_integration.DataSchemeVersionProperty); err != nil {
		return -1, err
	}

	if cds, ok := rdx.GetLastVal(vangogh_integration.DataSchemeVersionProperty, vangogh_integration.DataSchemeVersionProperty); ok && cds != "" {
		if cdi, err := strconv.ParseInt(cds, 10, 32); err == nil {
			return int(cdi), nil
		} else {
			return -1, err
		}
	}

	return 0, nil
}

func setLatestDataSchema(rdx redux.Writeable) error {
	if err := rdx.MustHave(vangogh_integration.DataSchemeVersionProperty); err != nil {
		return err
	}

	return rdx.ReplaceValues(vangogh_integration.DataSchemeVersionProperty,
		vangogh_integration.DataSchemeVersionProperty,
		strconv.FormatInt(latestDataSchema, 10))
}

// migrations

func removeDehydratedImageRepColor() error {

	reduxDir := vangogh_integration.AbsReduxDir()

	kvRedux, err := kevlar.New(reduxDir, kevlar.GobExt)
	if err != nil {
		return err
	}

	if err = kvRedux.Cut("dehydrated-image"); err != nil {
		return err
	}

	if err = kvRedux.Cut("rep-color"); err != nil {
		return err
	}

	return nil
}

func removeLargeTextProperties() error {

	reduxDir := vangogh_integration.AbsReduxDir()

	kvRedux, err := kevlar.New(reduxDir, kevlar.GobExt)
	if err != nil {
		return err
	}

	if err = kvRedux.Cut(vangogh_integration.DescriptionOverviewKeyValues); err != nil {
		return err
	}
	if err = kvRedux.Cut(vangogh_integration.DescriptionFeaturesKeyValues); err != nil {
		return err
	}
	if err = kvRedux.Cut(vangogh_integration.ChangelogKeyValues); err != nil {
		return err
	}

	return nil
}

func renameGogProductTypeDirectories() error {

	gogProductTypes := []vangogh_integration.ProductType{
		vangogh_integration.GogCatalogPage,
		vangogh_integration.GogAccountPage,
		vangogh_integration.GogUserWishlist,
		vangogh_integration.GogDetails,
		vangogh_integration.GogApiProducts,
		vangogh_integration.GogLicences,
		vangogh_integration.GogOrderPage,
		vangogh_integration.GogUserAccessToken,
	}

	for _, gpt := range gogProductTypes {
		newPtDir, err := vangogh_integration.AbsProductTypeDir(gpt)
		if err != nil {
			return err
		}

		existingPtDir := strings.TrimSuffix(newPtDir, gpt.String())
		existingPtDir = filepath.Join(existingPtDir, strings.TrimPrefix(gpt.String(), "gog-"))

		if _, err = os.Stat(existingPtDir); os.IsNotExist(err) {
			continue
		}

		if err = os.Rename(existingPtDir, newPtDir); err != nil {
			return err
		}
	}

	return nil
}

func removeUserAccessTokenValue() error {

	gogUatDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogUserAccessToken)
	if err != nil {
		return err
	}

	kvGogUat, err := kevlar.New(gogUatDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if kvGogUat.Has("user-access-token") {
		return kvGogUat.Cut("user-access-token")
	}

	return nil
}

func resetGetDataProperties() error {

	properties := []string{
		vangogh_integration.GetDataErrorDateProperty,
		vangogh_integration.GetDataErrorMessageProperty,
		vangogh_integration.GetDataLastUpdatedProperty,
	}

	reduxDir := vangogh_integration.AbsReduxDir()
	rdx, err := redux.NewWriter(reduxDir, properties...)
	if err != nil {
		return err
	}

	for _, p := range properties {

		allKeys := slices.Collect(rdx.Keys(p))

		if err = rdx.CutKeys(p, allKeys...); err != nil {
			return err
		}
	}

	return nil
}

func renameGogProperties() error {

	fromTo := make(map[string]string)

	toGogProperties := []string{
		vangogh_integration.GogLicencesProperty,
		vangogh_integration.GogUserWishlistProperty,
		vangogh_integration.GogAccountPageProductsProperty,
		vangogh_integration.GogCatalogPageProductsProperty,
		vangogh_integration.GogOrderPageProductsProperty,
		vangogh_integration.GogTitleProperty,
		vangogh_integration.GogOwnedProperty,
		vangogh_integration.GogDevelopersProperty,
		vangogh_integration.GogPublishersProperty,
		vangogh_integration.GogImageProperty,
		vangogh_integration.GogVerticalImageProperty,
		vangogh_integration.GogScreenshotsProperty,
		vangogh_integration.GogHeroProperty,
		vangogh_integration.GogLogoProperty,
		vangogh_integration.GogIconProperty,
		vangogh_integration.GogIconSquareProperty,
		vangogh_integration.GogBackgroundProperty,
		vangogh_integration.GogRatingProperty,
		vangogh_integration.GogProductTypeProperty,
		vangogh_integration.GogIncludesGamesProperty,
		vangogh_integration.GogIsIncludedByGamesProperty,
		vangogh_integration.GogRequiresGamesProperty,
		vangogh_integration.GogIsRequiredByGamesProperty,
		vangogh_integration.GogModifiesGamesProperty,
		vangogh_integration.GogIsModifiedByGamesProperty,
		vangogh_integration.GogEditionsProperty,
		vangogh_integration.GogRootEditionsProperty,
		vangogh_integration.GogStoreTagsProperty,
		vangogh_integration.GogGenresProperty,
		vangogh_integration.GogFeaturesProperty,
		vangogh_integration.GogSeriesProperty,
		vangogh_integration.GogThemesProperty,
		vangogh_integration.GogGameModesProperty,
		vangogh_integration.GogTagIdProperty,
		vangogh_integration.GogTagNameProperty,
		vangogh_integration.GogSlugProperty,
		vangogh_integration.GogGlobalReleaseDateProperty,
		vangogh_integration.GogManualUrlFilenameProperty,
		vangogh_integration.GogManualUrlStatusProperty,
		vangogh_integration.GogManualUrlValidationResultProperty,
		vangogh_integration.GogManualUrlGeneratedChecksumProperty,
		vangogh_integration.GogProductValidationResultProperty,
		vangogh_integration.GogProductGeneratedChecksumProperty,
		vangogh_integration.GogProductValidationDateProperty,
		vangogh_integration.GogStoreUrlProperty,
		vangogh_integration.GogForumUrlProperty,
		vangogh_integration.GogSupportUrlProperty,
		vangogh_integration.GogAdditionalRequirementsProperty,
		vangogh_integration.GogCopyrightsProperty,
		vangogh_integration.GogInDevelopmentProperty,
		vangogh_integration.GogPreOrderProperty,
		vangogh_integration.GogComingSoonProperty,
		vangogh_integration.GogBasePriceProperty,
		vangogh_integration.GogPriceProperty,
		vangogh_integration.GogIsFreeProperty,
		vangogh_integration.GogIsDemoProperty,
		vangogh_integration.GogIsModProperty,
		vangogh_integration.GogIsDiscountedProperty,
		vangogh_integration.GogDiscountPercentageProperty,
		vangogh_integration.GogSteamAppIdProperty,
		vangogh_integration.GogPcgwPageIdProperty,
		vangogh_integration.GogHltbIdProperty,
		vangogh_integration.GogIgdbIdProperty,
		vangogh_integration.GogStrategyWikiIdProperty,
		vangogh_integration.GogMobyGamesIdProperty,
		vangogh_integration.GogWikipediaIdProperty,
		vangogh_integration.GogWineHqIdProperty,
		vangogh_integration.GogVndbIdProperty,
		vangogh_integration.GogIgnWikiSlugProperty,
		vangogh_integration.GogOpenCriticIdProperty,
		vangogh_integration.GogOpenCriticSlugProperty,
	}

	for _, toProperty := range toGogProperties {
		fromProperty := strings.TrimPrefix(toProperty, "gog-")
		fromTo[fromProperty] = toProperty
	}

	return migrateFromToProperties(fromTo)
}

func migrateFromToProperties(fromTo map[string]string) error {

	rdxKv, err := kevlar.New(vangogh_integration.AbsReduxDir(), kevlar.GobExt)
	if err != nil {
		return err
	}

	for fromProperty, toProperty := range fromTo {

		if !rdxKv.Has(fromProperty) {
			continue
		}

		var rcFromProperty io.ReadCloser
		rcFromProperty, err = rdxKv.Get(fromProperty)
		if err != nil {
			return err
		}

		if err = rdxKv.Set(toProperty, rcFromProperty); err != nil {
			return err
		}

		if err = rcFromProperty.Close(); err != nil {
			return err
		}

		if err = rdxKv.Cut(fromProperty); err != nil {
			return err
		}
	}

	return nil
}
