package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/url"
	"os"
	"path/filepath"
	"slices"
	"strconv"
)

// Data scheme version log:
// 1. local-manual-url -> manual-url-filename
// 2. rm -rf metadata/_type_errors

const (
	latestDataSchema = 2
)

const DeprecatedTypeErrors pathways.RelDir = "_type_errors"

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

	typeErrorsDir := filepath.Join(metadataDir, string(DeprecatedTypeErrors))

	return os.RemoveAll(typeErrorsDir)
}
