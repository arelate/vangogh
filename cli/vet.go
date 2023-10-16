package cli

import (
	"github.com/arelate/vangogh/cli/dirs"
	"github.com/arelate/vangogh/cli/vets"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
)

const (
	VetOptionLocalOnlyData                = "local-only-data"
	VetOptionLocalOnlyImages              = "local-only-images"
	VetOptionLocalOnlyVideosAndThumbnails = "local-only-videos-and-thumbnails"
	VetOptionRecycleBin                   = "recycle-bin"
	VetOptionInvalidData                  = "invalid-data"
	VetOptionUnresolvedManualUrls         = "unresolved-manual-urls"
	VetOptionInvalidResolvedManualUrls    = "invalid-resolved-manual-urls"
	VetOptionMissingChecksums             = "missing-checksums"
	VetStaleDehydrations                  = "stale-dehydrations"
	VetOldLogs                            = "old-logs"
	VetOldBackups                         = "old-backups"
	VetWishlistedOwned                    = "wishlisted-owned"
)

type vetOptions struct {
	localOnlyData               bool
	localOnlyImages             bool
	localOnlyVideos             bool
	recycleBin                  bool
	invalidData                 bool
	unresolvedManualUrls        bool
	invalidUnresolvedManualUrls bool
	staleDehydrations           bool
	missingChecksums            bool
	oldLogs                     bool
	oldBackups                  bool
	wishlistedOwned             bool
}

func initVetOptions(u *url.URL) *vetOptions {

	vo := &vetOptions{
		localOnlyData:               vangogh_local_data.FlagFromUrl(u, VetOptionLocalOnlyData),
		localOnlyImages:             vangogh_local_data.FlagFromUrl(u, VetOptionLocalOnlyImages),
		localOnlyVideos:             vangogh_local_data.FlagFromUrl(u, VetOptionLocalOnlyVideosAndThumbnails),
		recycleBin:                  vangogh_local_data.FlagFromUrl(u, VetOptionRecycleBin),
		invalidData:                 vangogh_local_data.FlagFromUrl(u, VetOptionInvalidData),
		unresolvedManualUrls:        vangogh_local_data.FlagFromUrl(u, VetOptionUnresolvedManualUrls),
		invalidUnresolvedManualUrls: vangogh_local_data.FlagFromUrl(u, VetOptionInvalidResolvedManualUrls),
		missingChecksums:            vangogh_local_data.FlagFromUrl(u, VetOptionMissingChecksums),
		staleDehydrations:           vangogh_local_data.FlagFromUrl(u, VetStaleDehydrations),
		oldLogs:                     vangogh_local_data.FlagFromUrl(u, VetOldLogs),
		oldBackups:                  vangogh_local_data.FlagFromUrl(u, VetOldBackups),
		wishlistedOwned:             vangogh_local_data.FlagFromUrl(u, VetWishlistedOwned),
	}

	if vangogh_local_data.FlagFromUrl(u, "all") {
		vo.localOnlyData = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionLocalOnlyData))
		vo.localOnlyImages = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionLocalOnlyImages))
		vo.localOnlyVideos = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionLocalOnlyVideosAndThumbnails))
		vo.recycleBin = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionRecycleBin))
		vo.invalidData = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionInvalidData))
		vo.unresolvedManualUrls = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionUnresolvedManualUrls))
		vo.invalidUnresolvedManualUrls = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionInvalidResolvedManualUrls))
		vo.missingChecksums = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOptionMissingChecksums))
		vo.staleDehydrations = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetStaleDehydrations))
		vo.oldLogs = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOldLogs))
		vo.oldBackups = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetOldBackups))
		vo.wishlistedOwned = !vangogh_local_data.FlagFromUrl(u, NegOpt(VetWishlistedOwned))
	}

	return vo
}

func VetHandler(u *url.URL) error {

	vetOpts := initVetOptions(u)

	return Vet(
		vetOpts,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, "language-code"),
		vangogh_local_data.FlagFromUrl(u, "fix"))
}

func Vet(
	vetOpts *vetOptions,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string,
	fix bool) error {

	sda := nod.Begin("vetting local data...")
	defer sda.End()

	if vetOpts.localOnlyData {
		if err := vets.LocalOnlySplitProducts(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.localOnlyImages {
		if err := vets.LocalOnlyImages(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.localOnlyVideos {
		if err := vets.LocalOnlyVideosAndThumbnails(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.recycleBin {
		if err := vets.FilesInRecycleBin(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.invalidData {
		if err := vets.InvalidLocalProductData(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.unresolvedManualUrls {
		if err := vets.UnresolvedManualUrls(operatingSystems, downloadTypes, langCodes, fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.invalidUnresolvedManualUrls {
		if err := vets.InvalidResolvedManualUrls(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.missingChecksums {
		if err := vets.MissingChecksums(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.staleDehydrations {
		if err := StaleDehydrations(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.oldLogs {
		if err := vets.OldFiles(dirs.AbsLogsDir, "logs", fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.oldBackups {
		abp, err := vangogh_local_data.GetAbsDir(vangogh_local_data.Backups)
		if err != nil {
			return sda.EndWithError(err)
		}

		if err := vets.OldFiles(abp, "backups", fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	if vetOpts.wishlistedOwned {
		if err := WishlistedOwned(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	//products with values different from redux

	return nil
}
