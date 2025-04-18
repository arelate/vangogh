package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/vets"
	"github.com/boggydigital/nod"
	"net/url"
)

const (
	VetLocalOnlyImages = "local-only-images"
	VetRecycleBin      = "recycle-bin"
	VetOldLogs         = "old-logs"
)

type vetOptions struct {
	localOnlyImages bool
	recycleBin      bool
	oldLogs         bool
}

func initVetOptions(u *url.URL) *vetOptions {

	vo := &vetOptions{
		localOnlyImages: vangogh_integration.FlagFromUrl(u, VetLocalOnlyImages),
		recycleBin:      vangogh_integration.FlagFromUrl(u, VetRecycleBin),
		oldLogs:         vangogh_integration.FlagFromUrl(u, VetOldLogs),
	}

	if vangogh_integration.FlagFromUrl(u, "all") {
		vo.localOnlyImages = !vangogh_integration.FlagFromUrl(u, NegOpt(VetLocalOnlyImages))
		vo.recycleBin = !vangogh_integration.FlagFromUrl(u, NegOpt(VetRecycleBin))
		vo.oldLogs = !vangogh_integration.FlagFromUrl(u, NegOpt(VetOldLogs))
	}

	return vo
}

func VetHandler(u *url.URL) error {

	vetOpts := initVetOptions(u)

	return Vet(
		vetOpts,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
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

	if vetOpts.oldLogs {
		if err := vets.CleanupOldLogs(fix); err != nil {
			return err
		}
	}

	return nil
}
