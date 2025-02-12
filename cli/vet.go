package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/vets"
	"github.com/boggydigital/nod"
	"net/url"
)

const (
	VetLocalOnlyImages           = "local-only-images"
	VetRecycleBin                = "recycle-bin"
	VetInvalidData               = "invalid-data"
	VetUnresolvedManualUrls      = "unresolved-manual-urls"
	VetInvalidResolvedManualUrls = "invalid-resolved-manual-urls"
	VetMissingChecksums          = "missing-checksums"
	VetStaleDehydrations         = "stale-dehydrations"
	VetOldLogs                   = "old-logs"
)

type vetOptions struct {
	localOnlyImages             bool
	recycleBin                  bool
	invalidData                 bool
	unresolvedManualUrls        bool
	invalidUnresolvedManualUrls bool
	staleDehydrations           bool
	missingChecksums            bool
	oldLogs                     bool
}

func initVetOptions(u *url.URL) *vetOptions {

	vo := &vetOptions{
		localOnlyImages:             vangogh_integration.FlagFromUrl(u, VetLocalOnlyImages),
		recycleBin:                  vangogh_integration.FlagFromUrl(u, VetRecycleBin),
		invalidData:                 vangogh_integration.FlagFromUrl(u, VetInvalidData),
		unresolvedManualUrls:        vangogh_integration.FlagFromUrl(u, VetUnresolvedManualUrls),
		invalidUnresolvedManualUrls: vangogh_integration.FlagFromUrl(u, VetInvalidResolvedManualUrls),
		missingChecksums:            vangogh_integration.FlagFromUrl(u, VetMissingChecksums),
		staleDehydrations:           vangogh_integration.FlagFromUrl(u, VetStaleDehydrations),
		oldLogs:                     vangogh_integration.FlagFromUrl(u, VetOldLogs),
	}

	if vangogh_integration.FlagFromUrl(u, "all") {
		vo.localOnlyImages = !vangogh_integration.FlagFromUrl(u, NegOpt(VetLocalOnlyImages))
		vo.recycleBin = !vangogh_integration.FlagFromUrl(u, NegOpt(VetRecycleBin))
		vo.invalidData = !vangogh_integration.FlagFromUrl(u, NegOpt(VetInvalidData))
		vo.unresolvedManualUrls = !vangogh_integration.FlagFromUrl(u, NegOpt(VetUnresolvedManualUrls))
		vo.invalidUnresolvedManualUrls = !vangogh_integration.FlagFromUrl(u, NegOpt(VetInvalidResolvedManualUrls))
		vo.missingChecksums = !vangogh_integration.FlagFromUrl(u, NegOpt(VetMissingChecksums))
		vo.staleDehydrations = !vangogh_integration.FlagFromUrl(u, NegOpt(VetStaleDehydrations))
		vo.oldLogs = !vangogh_integration.FlagFromUrl(u, NegOpt(VetOldLogs))
	}

	return vo
}

func VetHandler(u *url.URL) error {

	vetOpts := initVetOptions(u)

	return Vet(
		vetOpts,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.FlagFromUrl(u, "fix"))
}

func Vet(
	vetOpts *vetOptions,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	fix bool) error {

	sda := nod.Begin("vetting local data...")
	defer sda.Done()

	if vetOpts.localOnlyImages {
		if err := vets.LocalOnlyImages(fix); err != nil {
			return err
		}
	}

	if vetOpts.recycleBin {
		if err := vets.FilesInRecycleBin(fix); err != nil {
			return err
		}
	}

	if vetOpts.invalidData {
		if err := vets.InvalidLocalProductData(fix); err != nil {
			return err
		}
	}

	if vetOpts.unresolvedManualUrls {
		if err := vets.UnresolvedManualUrls(operatingSystems, langCodes, downloadTypes, noPatches, fix); err != nil {
			return err
		}
	}

	if vetOpts.invalidUnresolvedManualUrls {
		if err := vets.InvalidResolvedManualUrls(fix); err != nil {
			return err
		}
	}

	if vetOpts.missingChecksums {
		if err := vets.MissingChecksums(operatingSystems, langCodes, noPatches, fix); err != nil {
			return err
		}
	}

	if vetOpts.staleDehydrations {
		if err := StaleDehydrations(fix); err != nil {
			return err
		}
	}

	if vetOpts.oldLogs {
		if err := vets.CleanupOldLogs(fix); err != nil {
			return err
		}
	}

	//products with values different from redux

	return nil
}
