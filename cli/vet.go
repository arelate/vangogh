package cli

import (
	"github.com/arelate/vangogh/cli/vets"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/url"
	"os"
	"strconv"
	"time"
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
		if err := vets.OldLogs(fix); err != nil {
			return sda.EndWithError(err)
		}
	}

	//products with values different from redux
	//logs older than 30 days
	//backups older than 30 days

	return nil
}

// StaleDehydrations needs to be in cli to avoid import cycle
func StaleDehydrations(fix bool) error {

	if err := staleDehydrationsImageType(
		vangogh_local_data.ImageProperty,
		vangogh_local_data.DehydratedImageModifiedProperty, fix); err != nil {
		return err
	}

	if err := staleDehydrationsImageType(
		vangogh_local_data.VerticalImageProperty,
		vangogh_local_data.DehydratedVerticalImageModifiedProperty, fix); err != nil {
		return err
	}

	return nil
}

func staleDehydrationsImageType(imageProperty, dimProperty string, fix bool) error {

	sdia := nod.NewProgress("checking stale dehydrations for %s...", imageProperty)
	defer sdia.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(imageProperty, dimProperty)
	if err != nil {
		return err
	}

	staleIds := make(map[string]bool)

	ids := rxa.Keys(imageProperty)
	sdia.TotalInt(len(ids))

	for _, id := range ids {
		if imageId, ok := rxa.GetFirstVal(imageProperty, id); ok {
			imagePath := vangogh_local_data.AbsLocalImagePath(imageId)
			if stat, err := os.Stat(imagePath); err == nil {
				if dimStr, ok := rxa.GetFirstVal(dimProperty, id); ok {
					if dim, err := strconv.ParseInt(dimStr, 10, 64); err == nil {
						dimTime := time.Unix(dim, 0)
						imt := stat.ModTime()
						if imt.After(dimTime) {
							staleIds[id] = true
						}
					}
				}
			}
		}
		sdia.Increment()
	}

	if len(staleIds) == 0 {
		sdia.EndWithResult("all good")
	} else {
		sdia.EndWithResult("found %d stale dehydrations", len(staleIds))
		if fix {
			imageType := vangogh_local_data.ImageTypeFromProperty(imageProperty)
			return Dehydrate(staleIds, []vangogh_local_data.ImageType{imageType}, false)
		}

	}

	return nil
}
