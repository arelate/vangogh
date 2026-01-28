package cli

import (
	"net/url"
	"os"
	"path/filepath"
	"strconv"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
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

const (
	latestDataSchema = 9
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
		case 7:
			if err = cutPreviouslyCascadedValidations(); err != nil {
				return err
			}
		case 8:
			if err = fixDlcExtrasDownloads(); err != nil {
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

func cutPreviouslyCascadedValidations() error {

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.ReduxProperties()...)
	if err != nil {
		return err
	}

	packDlcValidations := make([]string, 0)

	for id := range rdx.Keys(vangogh_integration.ProductValidationResultProperty) {

		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok && pt == vangogh_integration.GameProductType {
			continue
		}

		packDlcValidations = append(packDlcValidations, id)
	}

	return rdx.CutKeys(vangogh_integration.ProductValidationResultProperty, packDlcValidations...)
}

func fixDlcExtrasDownloads() error {

	fdeda := nod.Begin(" fixing DLCs, Extras directories, please wait...")
	defer fdeda.Done()

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(), vangogh_integration.SlugProperty)
	if err != nil {
		return err
	}

	for id := range kvDetails.Keys() {

		var slug string
		if sp, ok := rdx.GetLastVal(vangogh_integration.SlugProperty, id); ok && sp != "" {
			slug = sp
		}

		if slug == "" {
			continue
		}

		for _, dl := range []vangogh_integration.DownloadsLayout{vangogh_integration.FlatDownloadsLayout, vangogh_integration.ShardedDownloadsLayout} {

			var slugDownloadsDir string
			slugDownloadsDir, err = vangogh_integration.AbsSlugDownloadDir(slug, vangogh_integration.Installer, dl)

			for _, dt := range []vangogh_integration.DownloadType{vangogh_integration.DLC, vangogh_integration.Extra} {

				if err = fixSlugDtDlDownloads(slug, slugDownloadsDir, dt, dl); err != nil {
					return err
				}
			}
		}
	}

	return nil
}

func fixSlugDtDlDownloads(slug, slugDownloadsDir string, dt vangogh_integration.DownloadType, dl vangogh_integration.DownloadsLayout) error {
	var rd pathways.RelDir
	switch dt {
	case vangogh_integration.DLC:
		rd = vangogh_integration.DLCs
	case vangogh_integration.Extra:
		rd = vangogh_integration.Extras
	default:
		// do nothing
	}

	if rd == "" {
		return nil
	}

	wrongRelDir := vangogh_integration.Pwd.AbsRelDirPath(rd, vangogh_integration.Downloads)

	absWrongPath := filepath.Join(slugDownloadsDir, wrongRelDir)

	if _, err := os.Stat(absWrongPath); err == nil {

		problem := nod.Begin(" - found %s...", absWrongPath)
		defer problem.Done()

		var absCorrectPath string
		absCorrectPath, err = vangogh_integration.AbsSlugDownloadDir(slug, dt, dl)
		if err != nil {
			return nil
		}

		if _, err = os.Stat(absCorrectPath); err == nil {

			if err = os.RemoveAll(absWrongPath); err != nil {
				return err
			}

			problem.EndWithResult("removed")

		} else if os.IsNotExist(err) {

			if err = os.Rename(absWrongPath, absCorrectPath); err != nil {
				return nil
			}

			problem.EndWithResult("moved to %s", absCorrectPath)

		} else {
			return err
		}

	} else if os.IsNotExist(err) {
		// do nothing
	} else {
		return err
	}

	return removeRemaingWrongDownloadsSubDirs(absWrongPath, slugDownloadsDir)
}

func removeRemaingWrongDownloadsSubDirs(absWrongPath, slugDownloadsDir string) error {

	path := absWrongPath
	for {

		absWrongSubdir := filepath.Dir(path)

		if len(absWrongSubdir) <= len(slugDownloadsDir) {
			break
		}

		if _, err := os.Stat(absWrongSubdir); err == nil {
			if err = os.RemoveAll(absWrongSubdir); err != nil {
				return err
			}

		} else if os.IsNotExist(err) {
			// do nothing
		}

		path = absWrongSubdir
	}
	return nil
}
