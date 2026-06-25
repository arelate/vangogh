package cli

import (
	"io"
	"net/url"
	"strconv"

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
// 16. rename misc properties

const (
	latestDataSchema = 17
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
		vangogh_integration.VangoghDataSchemeVersionProperty)
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
		case 16:
			if err = renameMiscProperties(); err != nil {
				return err
			}
		}

		mda.Increment()
	}

	rdx, err = redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.VangoghDataSchemeVersionProperty)
	if err != nil {
		return err
	}

	return setLatestDataSchema(rdx)
}

func getCurrentDataSchema(rdx redux.Readable) (int, error) {

	if err := rdx.MustHave(vangogh_integration.VangoghDataSchemeVersionProperty); err != nil {
		return -1, err
	}

	if cds, ok := rdx.GetLastVal(vangogh_integration.VangoghDataSchemeVersionProperty, vangogh_integration.VangoghDataSchemeVersionProperty); ok && cds != "" {
		if cdi, err := strconv.ParseInt(cds, 10, 32); err == nil {
			return int(cdi), nil
		} else {
			return -1, err
		}
	}

	return 0, nil
}

func setLatestDataSchema(rdx redux.Writeable) error {
	if err := rdx.MustHave(vangogh_integration.VangoghDataSchemeVersionProperty); err != nil {
		return err
	}

	return rdx.ReplaceValues(vangogh_integration.VangoghDataSchemeVersionProperty,
		vangogh_integration.VangoghDataSchemeVersionProperty,
		strconv.FormatInt(latestDataSchema, 10))
}

// migrations

func renameMiscProperties() error {

	fromTo := map[string]string{
		"gog-account-page-products":          vangogh_integration.GogAccountProductPageProperty,
		"gog-catalog-page-products":          vangogh_integration.GogCatalogProductPageProperty,
		"gog-tag":                            vangogh_integration.GogTagIdProperty,
		"local-tags":                         vangogh_integration.VangoghLocalTagsProperty,
		"download-status-error":              vangogh_integration.VangoghDownloadStatusErrorProperty,
		"download-queued":                    vangogh_integration.VangoghDownloadQueuedProperty,
		"download-started":                   vangogh_integration.VangoghDownloadStartedProperty,
		"download-completed":                 vangogh_integration.VangoghDownloadCompletedProperty,
		"summary-rating":                     vangogh_integration.VangoghSummaryRatingProperty,
		"summary-reviews":                    vangogh_integration.VangoghSummaryReviewsProperty,
		"video-id":                           vangogh_integration.GogYouTubeVideoIdProperty,
		"video-title":                        vangogh_integration.YouTubeVideoTitleProperty,
		"video-duration":                     vangogh_integration.YouTubeVideoDurationProperty,
		"video-error":                        vangogh_integration.YouTubeVideoErrorProperty,
		"steamos-app-compatibility-category": vangogh_integration.SteamSteamOsAppCompatibilityCategoryProperty,
		"sync-events":                        vangogh_integration.VangoghSyncEventsProperty,
		"last-sync-updates":                  vangogh_integration.VangoghLastSyncUpdatesProperty,
		"data-scheme-version":                vangogh_integration.VangoghDataSchemeVersionProperty,
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
