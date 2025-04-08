package cli

import (
	"errors"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/url"
	"os"
)

func RelayoutDownloadsHandler(u *url.URL) error {

	q := u.Query()

	operatingSystems := vangogh_integration.OperatingSystemsFromUrl(u)
	langCodes := vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty)
	downloadTypes := vangogh_integration.DownloadTypesFromUrl(u)
	noPatches := vangogh_integration.FlagFromUrl(u, "no-patches")

	from := vangogh_integration.ParseDownloadsLayout(q.Get("from"))
	to := vangogh_integration.ParseDownloadsLayout(q.Get("to"))

	return RelayoutDownloads(operatingSystems, langCodes, downloadTypes, noPatches, from, to)
}

func RelayoutDownloads(
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	from, to vangogh_integration.DownloadsLayout) error {

	rda := nod.NewProgress("changing downloads layout from %s to %s...", from, to)
	defer rda.Done()

	if err := Backup(); err != nil {
		return err
	}

	if from == vangogh_integration.UnknownDownloadsLayout ||
		to == vangogh_integration.UnknownDownloadsLayout ||
		from == to {
		return errors.New("from and to downloads layouts must be valid and different")
	}

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	var ids []string
	for id := range kvDetails.Keys() {
		ids = append(ids, id)
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.SlugProperty,
		vangogh_integration.LocalManualUrlProperty)
	if err != nil {
		return err
	}

	drp := &downloadsRelayoutProcessor{
		rdx:  rdx,
		from: from,
		to:   to,
	}

	if err = vangogh_integration.MapDownloads(ids, rdx, operatingSystems, langCodes, downloadTypes, noPatches, drp, rda); err != nil {
		return err
	}

	return nil
}

type downloadsRelayoutProcessor struct {
	rdx      redux.Writeable
	from, to vangogh_integration.DownloadsLayout
}

func (drp *downloadsRelayoutProcessor) Process(_ string, slug string, downloadsList vangogh_integration.DownloadsList) error {

	fromDir, err := vangogh_integration.AbsProductDownloadsDir(slug, drp.from)
	if err != nil {
		return err
	}

	if _, err = os.Stat(fromDir); err != nil {
		return err
	}

	toDir, err := vangogh_integration.AbsProductDownloadsDir(slug, drp.to)
	if err != nil {
		return err
	}

	// checking for err == nil to make sure destination directory does NOT exist
	if _, err = os.Stat(toDir); err == nil {
		return errors.New("destination layout directory already exist: " + toDir)
	}

	if err = os.Rename(fromDir, toDir); err != nil {
		return err
	}

	// update manual-url -> local file associations
	for _, dl := range downloadsList {
		if err = replaceLocalManualUrl(&dl, slug, drp.rdx, drp.to); err != nil {
			return err
		}
	}

	return nil
}
