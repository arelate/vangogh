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
	"path/filepath"
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

	rda := nod.Begin("changing downloads layout from %s to %s...", from, to)
	defer rda.Done()

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
		from: from,
		to:   to,
	}

	mda := nod.NewProgress(" moving downloads directories to a new layout...")
	if err = vangogh_integration.MapDownloads(ids, rdx, operatingSystems, langCodes, downloadTypes, noPatches, drp, mda); err != nil {
		return err
	}
	mda.Done()

	rrp := &reduxRelayoutProcessor{
		rdx:             rdx,
		localManualUrls: make(map[string][]string),
		to:              to,
	}

	ulmupa := nod.NewProgress(" updating %s...", vangogh_integration.LocalManualUrlProperty)
	if err = vangogh_integration.MapDownloads(ids, rdx, operatingSystems, langCodes, downloadTypes, noPatches, rrp, ulmupa); err != nil {
		return err
	}
	ulmupa.Done()

	if err = rdx.BatchReplaceValues(vangogh_integration.LocalManualUrlProperty, rrp.localManualUrls); err != nil {
		return err
	}

	return nil
}

type downloadsRelayoutProcessor struct {
	from, to vangogh_integration.DownloadsLayout
}

func (drp *downloadsRelayoutProcessor) Process(_ string, slug string, _ vangogh_integration.DownloadsList) error {

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

	return nil
}

type reduxRelayoutProcessor struct {
	rdx             redux.Readable
	localManualUrls map[string][]string
	to              vangogh_integration.DownloadsLayout
}

func (rrp *reduxRelayoutProcessor) Process(_ string, slug string, downloadsList vangogh_integration.DownloadsList) error {

	productRelDir, err := vangogh_integration.RelProductDownloadsDir(slug, rrp.to)

	for _, dl := range downloadsList {
		relDownloadTypeDir := ""
		switch dl.Type {
		case vangogh_integration.DLC:
			relDownloadTypeDir, err = pathways.GetRelDir(vangogh_integration.DLCs)
		case vangogh_integration.Extra:
			relDownloadTypeDir, err = pathways.GetRelDir(vangogh_integration.Extras)
		default:
			// do nothing - use base product downloads dir
		}
		if err != nil {
			return err
		}

		relDir := filepath.Join(productRelDir, relDownloadTypeDir)

		if relLocalFilename, ok := rrp.rdx.GetLastVal(vangogh_integration.LocalManualUrlProperty, dl.ManualUrl); ok && relLocalFilename != "" {
			_, filename := filepath.Split(relLocalFilename)
			rrp.localManualUrls[dl.ManualUrl] = []string{filepath.Join(relDir, filename)}
		}

	}

	return nil
}
