package cli

import (
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"net/url"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

const (
	SyncOptionPurchases         = "purchases"
	SyncOptionDescriptionImages = "description-images"
	SyncOptionImages            = "images"
	SyncOptionScreenshots       = "screenshots"
	SyncOptionVideosMetadata    = "videos-metadata"
	SyncOptionDownloadsUpdates  = "downloads-updates"
	SynOptionWineBinaries       = "wine-binaries"
	negativePrefix              = "no-"
)

type syncOptions struct {
	purchases         bool
	descriptionImages bool
	images            bool
	screenshots       bool
	videosMetadata    bool
	downloadsUpdates  bool
	wineBinaries      bool
}

func NegOpt(option string) string {
	if !strings.HasPrefix(option, negativePrefix) {
		return negativePrefix + option
	}
	return option
}

func initSyncOptions(u *url.URL) *syncOptions {

	so := &syncOptions{
		purchases:         vangogh_integration.FlagFromUrl(u, SyncOptionPurchases),
		descriptionImages: vangogh_integration.FlagFromUrl(u, SyncOptionDescriptionImages),
		images:            vangogh_integration.FlagFromUrl(u, SyncOptionImages),
		screenshots:       vangogh_integration.FlagFromUrl(u, SyncOptionScreenshots),
		videosMetadata:    vangogh_integration.FlagFromUrl(u, SyncOptionVideosMetadata),
		downloadsUpdates:  vangogh_integration.FlagFromUrl(u, SyncOptionDownloadsUpdates),
		wineBinaries:      vangogh_integration.FlagFromUrl(u, SynOptionWineBinaries),
	}

	if vangogh_integration.FlagFromUrl(u, "all") {
		// not handling purchases here - doesn't make sense to negate it
		so.descriptionImages = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionDescriptionImages))
		so.images = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionImages))
		so.screenshots = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionScreenshots))
		so.videosMetadata = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionVideosMetadata))
		so.downloadsUpdates = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionDownloadsUpdates))
		so.wineBinaries = !vangogh_integration.FlagFromUrl(u, NegOpt(SynOptionWineBinaries))
	}

	return so
}

func SyncHandler(u *url.URL) error {
	syncOpts := initSyncOptions(u)

	since, err := vangogh_integration.SinceFromUrl(u)
	if err != nil {
		return err
	}

	debug := vangogh_integration.FlagFromUrl(u, "debug")

	force := u.Query().Has("force")

	cleanup := !vangogh_integration.FlagFromUrl(u, "no-cleanup")

	return Sync(
		since,
		syncOpts,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		cleanup,
		debug,
		force)
}

func Sync(
	since int64,
	syncOpts *syncOptions,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_integration.DownloadType,
	noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	cleanup bool,
	debug, force bool) error {

	if debug {
		absLogsDir, err := pathways.GetAbsDir(vangogh_integration.Logs)
		if err != nil {
			return err
		}
		logger, err := nod.EnableFileLogger(absLogsDir)
		if err != nil {
			return err
		}
		defer logger.Close()
	}

	sa := nod.Begin("syncing source data...")
	defer sa.Done()

	syncStart := since
	if syncStart == 0 {
		syncStart = time.Now().Unix()
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	syncEventsRdx, err := redux.NewWriter(reduxDir, vangogh_integration.SyncEventsProperty)
	if err != nil {
		return err
	}

	syncEvents := make(map[string][]string, 2)
	syncEvents[vangogh_integration.SyncStartKey] = []string{strconv.Itoa(int(syncStart))}

	if err = syncEventsRdx.BatchReplaceValues(vangogh_integration.SyncEventsProperty, syncEvents); err != nil {
		return err
	}

	if err = GetData(nil, nil, since, syncOpts.purchases, true, force); err != nil {
		return err
	}

	if err = Reduce([]vangogh_integration.ProductType{vangogh_integration.UnknownProductType}); err != nil {
		return err
	}

	// summarize sync updates now, since other updates are digital artifacts
	// and won't affect the summaries
	if err = Summarize(syncStart); err != nil {
		return err
	}

	// get description images
	if syncOpts.descriptionImages {
		if err = GetDescriptionImages(nil, since, force); err != nil {
			return err
		}
	}

	// get images
	if syncOpts.images {
		imageTypes := make([]vangogh_integration.ImageType, 0, len(vangogh_integration.AllImageTypes()))
		for _, it := range vangogh_integration.AllImageTypes() {
			if !syncOpts.screenshots && it == vangogh_integration.Screenshots {
				continue
			}
			imageTypes = append(imageTypes, it)
		}
		if err = GetImages(nil, imageTypes, true, force); err != nil {
			return err
		}

		dehydratedImageTypes := []vangogh_integration.ImageType{vangogh_integration.Image, vangogh_integration.VerticalImage}
		if err = Dehydrate(nil, dehydratedImageTypes, false); err != nil {
			return err
		}
	}

	if syncOpts.videosMetadata {
		if err = GetVideoMetadata(nil, true, false); err != nil {
			return err
		}
	}

	// get downloads updates
	if syncOpts.downloadsUpdates {

		updatedDetails, err := shared_data.GetDetailsUpdates(since)
		if err != nil {
			return err
		}

		if err = queueDownloads(maps.Keys(updatedDetails)); err != nil {
			return err
		}

		// process remaining queued downloads, e.g. downloads that were queued before this sync
		// and were not completed successfully due to an interruption. Download updates
		// itemized earlier in the sync cycle (ids) are intentionally excluded to
		// focus on previously queued and avoid attempting to download problematic ids
		// right after they didn't download successfully, waiting until the next sync
		// is likely a better strategy in that case
		if err = ProcessQueue(
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches,
			downloadsLayout); err != nil {
			return err
		}

		if cleanup {
			if err = Cleanup(nil,
				operatingSystems,
				langCodes,
				downloadTypes,
				noPatches,
				downloadsLayout,
				true,
				false); err != nil {
				return err
			}
		}

	}

	if syncOpts.wineBinaries {
		if err = GetWineBinaries(operatingSystems, force); err != nil {
			return err
		}
	}

	syncEvents[vangogh_integration.SyncCompleteKey] = []string{strconv.Itoa(int(time.Now().Unix()))}

	if err = syncEventsRdx.BatchReplaceValues(vangogh_integration.SyncEventsProperty, syncEvents); err != nil {
		return err
	}

	// backing up data
	if err = Backup(); err != nil {
		return err
	}

	// print new, updated
	return GetSummary()
}
