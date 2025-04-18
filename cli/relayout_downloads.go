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
	langCodes := vangogh_integration.LanguageCodesFromUrl(u)
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
		vangogh_integration.SlugProperty)
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

	if len(drp.errs) > 0 {
		joinedErrs := errors.Join(drp.errs...)
		rda.EndWithResult("encountered %d error(s) while moving directories: %s", len(drp.errs), joinedErrs)
	}

	return nil
}

type downloadsRelayoutProcessor struct {
	rdx      redux.Readable
	errs     []error
	from, to vangogh_integration.DownloadsLayout
}

func (drp *downloadsRelayoutProcessor) Process(_ string, slug string, downloadsList vangogh_integration.DownloadsList) error {

	if len(downloadsList) == 0 {
		return nil
	}

	fromDir, err := vangogh_integration.AbsSlugDownloadDir(slug, vangogh_integration.Installer, drp.from)
	if err != nil {
		return err
	}

	if _, err = os.Stat(fromDir); err != nil {
		drp.errs = append(drp.errs, err)
		return nil
	}

	toDir, err := vangogh_integration.AbsSlugDownloadDir(slug, vangogh_integration.Installer, drp.to)
	if err != nil {
		return err
	}

	// currently this is required for sharded layout, but is a good practice to check in general
	parentToDir, _ := filepath.Split(toDir)
	if _, err = os.Stat(parentToDir); os.IsNotExist(err) {
		if err = os.MkdirAll(parentToDir, 0755); err != nil {
			return err
		}
	}

	// checking for err == nil to make sure destination directory does NOT exist
	if _, err = os.Stat(toDir); err == nil {
		err = errors.New("destination layout directory already exist: " + toDir)
		drp.errs = append(drp.errs, err)
		return nil
	}

	if err = os.Rename(fromDir, toDir); err != nil {
		drp.errs = append(drp.errs, err)
		return nil
	}

	if drp.from == vangogh_integration.ShardedDownloadsLayout {
		shardDir, _ := filepath.Split(fromDir)
		if err = removeDirIfEmpty(shardDir); err != nil {
			return err
		}
	}

	return nil
}
