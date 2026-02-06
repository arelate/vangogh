package cli

import (
	"errors"
	"net/url"
	"os"
	"path/filepath"
	"slices"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GenerateMissingChecksumsHandler(u *url.URL) error {

	q := u.Query()

	operatingSystems := vangogh_integration.OperatingSystemsFromUrl(u)
	langCodes := vangogh_integration.LanguageCodesFromUrl(u)
	downloadTypes := vangogh_integration.DownloadTypesFromUrl(u)
	noPatches := q.Has("no-patches")
	downloadsLayout := vangogh_integration.DownloadsLayoutFromUrl(u)

	return GenerateMissingChecksums(operatingSystems, langCodes, downloadTypes, noPatches, downloadsLayout)
}

func GenerateMissingChecksums(operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout) error {

	gmca := nod.NewProgress("generating missing checksums...")
	defer gmca.Done()

	reduxDir := vangogh_integration.AbsReduxDir()

	properties := append(
		vangogh_integration.DownloadsLifecycleProperties(),
		vangogh_integration.ProductGeneratedChecksumProperty,
		vangogh_integration.ManualUrlGeneratedChecksumProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.SlugProperty)

	rdx, err := redux.NewWriter(reduxDir, properties...)
	if err != nil {
		return err
	}

	productsMissingChecksumsQuery := map[string][]string{
		vangogh_integration.ProductValidationResultProperty: {vangogh_integration.ValidationStatusMissingChecksum.String()},
	}

	ids := slices.Collect(rdx.Match(productsMissingChecksumsQuery, redux.FullMatch))

	mcp := &missingChecksumProcessor{
		rdx:             rdx,
		downloadsLayout: downloadsLayout,
	}

	if err = vangogh_integration.MapDownloads(ids,
		rdx,
		operatingSystems,
		langCodes,
		downloadTypes,
		noPatches,
		mcp,
		gmca); err != nil {
		return err
	}

	return nil
}

type missingChecksumProcessor struct {
	rdx             redux.Writeable
	downloadsLayout vangogh_integration.DownloadsLayout
}

func (mcp *missingChecksumProcessor) Process(id string, slug string, downloadsList vangogh_integration.DownloadsList) error {

	for _, dl := range downloadsList {

		if vrs, ok := mcp.rdx.GetLastVal(vangogh_integration.ManualUrlValidationResultProperty, dl.ManualUrl); ok && vrs != "" {
			if vr := vangogh_integration.ParseValidationStatus(vrs); vr == vangogh_integration.ValidationStatusMissingChecksum {
				if err := mcp.generateMissingChecksum(slug, dl.ManualUrl, dl.DownloadType); err != nil {
					return err
				}
			}
		}
	}

	if err := mcp.rdx.ReplaceValues(vangogh_integration.ProductGeneratedChecksumProperty, id, vangogh_integration.TrueValue); err != nil {
		return err
	}

	if err := mcp.rdx.ReplaceValues(vangogh_integration.ProductValidationResultProperty, id, vangogh_integration.ValidationStatusSuccess.String()); err != nil {
		return err
	}

	return nil
}

func (mcp *missingChecksumProcessor) generateMissingChecksum(slug, manualUrl string, dt vangogh_integration.DownloadType) error {

	absSlugDownloadDir, err := vangogh_integration.AbsSlugDownloadDir(slug, dt, mcp.downloadsLayout)
	if err != nil {
		return err
	}

	var relFilename string
	if fn, ok := mcp.rdx.GetLastVal(vangogh_integration.ManualUrlFilenameProperty, manualUrl); ok && fn != "" {
		relFilename = fn
	}

	if relFilename == "" {
		return errors.New("unresolved local filename for " + manualUrl)
	}

	absDownloadFilename := filepath.Join(absSlugDownloadDir, relFilename)

	stat, err := os.Stat(absDownloadFilename)
	if err != nil {
		return err
	}

	gcfa := nod.NewProgress(" - %s", relFilename)
	gcfa.Total(uint64(stat.Size()))
	defer gcfa.Done()

	downloadMd5, err := hashPathMd5(absDownloadFilename, gcfa)
	if err != nil {
		return err
	}

	gcfa.EndWithResult(downloadMd5)

	if err = mcp.rdx.ReplaceValues(vangogh_integration.ManualUrlGeneratedChecksumProperty, manualUrl, downloadMd5); err != nil {
		return err
	}

	if err = mcp.rdx.ReplaceValues(vangogh_integration.ManualUrlValidationResultProperty, manualUrl, vangogh_integration.ValidationStatusSuccess.String()); err != nil {
		return err
	}

	return nil
}
