package cli

import (
	"net/url"
	"os"
	"path/filepath"
	"slices"
	"strconv"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

// Data scheme version log:
// 1. local-manual-url -> manual-url-filename
// 2. rm -rf metadata/_type_errors
// 3. rename updates sections (remove old section titles)
// 4. rename _binaries to _wine-binaries
// 5. deprecate aggregated-rating property
// 6. rename logs from year-month-date-hour-minute-second.log to sync-year-month-date-hour-minute-second.log
// 7. catalog-page now uses 100 products limit (vs 48) so there's fewer pages (extra ones need to be removed)

const (
	latestDataSchema = 7
)

const deprecatedTypeErrors pathways.RelDir = "_type_errors"

const (
	installersUpdatesTitle = "Installers updates"
	newProductTitle        = "New products"
	releasedTodayTitle     = "Released today"
	steamNewsTitle         = "Steam news"
)

const (
	deprecatedBinaries pathways.RelDir = "_binaries"
)

const (
	aggregatedRatingProperty = "aggregated-rating"
)

const (
	currentCatalogPages = 113
)

func MigrateDataHandler(u *url.URL) error {
	return MigrateData(u.Query().Has("force"))
}

func MigrateData(force bool) error {
	mda := nod.NewProgress("migrating data to the latest schema...")
	defer mda.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.DataSchemeVersionProperty)
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
		case 0:
			if err = migrateLocalManualUrlToManualUrlFilename(); err != nil {
				return err
			}
		case 1:
			if err = removeTypeErrorsDir(); err != nil {
				return err
			}
		case 2:
			if err = removeLiteralUpdatesSections(); err != nil {
				return err
			}
		case 3:
			if err = renameBinariesToWineBinaries(); err != nil {
				return err
			}
		case 4:
			if err = deprecateAggregatedRatingProperty(); err != nil {
				return err
			}
		case 5:
			if err = removeOldLogs(); err != nil {
				return err
			}
		case 6:
			if err = removeExtraCatalogPages(); err != nil {
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

func migrateLocalManualUrlToManualUrlFilename() error {

	ma := nod.Begin(" migrating %s to %s...", vangogh_integration.LocalManualUrlProperty, vangogh_integration.ManualUrlFilenameProperty)
	defer ma.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.LocalManualUrlProperty,
		vangogh_integration.ManualUrlFilenameProperty)
	if err != nil {
		return err
	}

	// 1) replace manual-url-filename filename values from local-manual-url
	// 2) cut all local-manual-url values

	// 1)
	manualUrlFilenames := make(map[string][]string)
	for id := range rdx.Keys(vangogh_integration.LocalManualUrlProperty) {
		if lmu, ok := rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, id); ok && lmu != "" {
			_, filename := filepath.Split(lmu)
			manualUrlFilenames[id] = []string{filename}
		}
	}

	if err = rdx.BatchReplaceValues(vangogh_integration.ManualUrlFilenameProperty, manualUrlFilenames); err != nil {
		return err
	}

	// 2)
	if err = rdx.CutKeys(vangogh_integration.LocalManualUrlProperty,
		slices.Collect(rdx.Keys(vangogh_integration.LocalManualUrlProperty))...); err != nil {
		return err
	}

	return nil
}

func removeTypeErrorsDir() error {

	metadataDir, err := pathways.GetAbsDir(vangogh_integration.Metadata)
	if err != nil {
		return err
	}

	typeErrorsDir := filepath.Join(metadataDir, string(deprecatedTypeErrors))

	return os.RemoveAll(typeErrorsDir)
}

func removeLiteralUpdatesSections() error {
	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.LastSyncUpdatesProperty)
	if err != nil {
		return err
	}

	if err = rdx.CutKeys(vangogh_integration.LastSyncUpdatesProperty,
		installersUpdatesTitle,
		newProductTitle,
		releasedTodayTitle,
		steamNewsTitle); err != nil {
		return err
	}

	return err
}

func renameBinariesToWineBinaries() error {

	wineBinariesDir, err := pathways.GetAbsRelDir(vangogh_integration.WineBinaries)
	if err != nil {
		return err
	}

	if _, err = os.Stat(wineBinariesDir); err == nil {
		return nil
	}

	downloadsDir, err := pathways.GetAbsDir(vangogh_integration.Downloads)
	if err != nil {
		return err
	}

	deprecatedBinariesDir := filepath.Join(downloadsDir, string(deprecatedBinaries))

	return os.Rename(deprecatedBinariesDir, wineBinariesDir)
}

func deprecateAggregatedRatingProperty() error {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	return os.Remove(filepath.Join(reduxDir, aggregatedRatingProperty+kevlar.GobExt))
}

func removeOldLogs() error {

	logsDir, err := pathways.GetAbsDir(vangogh_integration.Logs)
	if err != nil {
		return err
	}

	ld, err := os.Open(logsDir)
	if err != nil {
		return err
	}

	logs, err := ld.Readdirnames(-1)
	if err != nil {
		return err
	}

	for _, logName := range logs {
		if strings.HasPrefix(logName, "sync") || !strings.HasSuffix(logName, ".log") {
			continue
		}

		absLogPath := filepath.Join(logsDir, logName)
		if err = os.Remove(absLogPath); err != nil {
			return err
		}
	}

	return nil
}

func removeExtraCatalogPages() error {

	catalogPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.CatalogPage)
	if err != nil {
		return err
	}

	kvCatalogPages, err := kevlar.New(catalogPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	for ii := 1; ii <= kvCatalogPages.Len(); ii++ {
		if ii <= currentCatalogPages {
			continue
		}
		page := strconv.Itoa(ii)
		if kvCatalogPages.Has(page) {
			if err = kvCatalogPages.Cut(page); err != nil {
				return err
			}
		}
	}

	return nil
}
