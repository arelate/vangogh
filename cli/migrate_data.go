package cli

import (
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
// 9. internal version
// 10. remove dehydrated-image, rep-color
// 11. remove description-overview, description-features, changelog and screenshots properties // 1.2.7

const (
	latestDataSchema = 12
)

func MigrateDataHandler(u *url.URL) error {
	return MigrateData(u.Query().Has("force"))
}

func MigrateData(force bool) error {
	mda := nod.NewProgress("migrating data to the latest schema...")
	defer mda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.DataSchemeVersionProperty)
	if err != nil {
		return err
	}

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
			if err = removeLargeProperties(); err != nil {
				return err
			}
		}

		mda.Increment()
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

func removeLargeProperties() error {

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
	if err = kvRedux.Cut(vangogh_integration.ScreenshotsKeyValues); err != nil {
		return err
	}

	return nil
}
