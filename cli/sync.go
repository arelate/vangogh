package cli

import (
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/pathways"
	"maps"
	"net/url"
	"slices"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

const (
	SyncOptionItems            = "items"
	SyncOptionImages           = "images"
	SyncOptionScreenshots      = "screenshots"
	SyncOptionVideosMetadata   = "videos-metadata"
	SyncOptionDownloadsUpdates = "downloads-updates"
	negativePrefix             = "no-"
)

type syncOptions struct {
	items            bool
	images           bool
	screenshots      bool
	videosMetadata   bool
	downloadsUpdates bool
}

func NegOpt(option string) string {
	if !strings.HasPrefix(option, negativePrefix) {
		return negativePrefix + option
	}
	return option
}

func initSyncOptions(u *url.URL) *syncOptions {

	so := &syncOptions{
		items:            vangogh_integration.FlagFromUrl(u, SyncOptionItems),
		images:           vangogh_integration.FlagFromUrl(u, SyncOptionImages),
		screenshots:      vangogh_integration.FlagFromUrl(u, SyncOptionScreenshots),
		videosMetadata:   vangogh_integration.FlagFromUrl(u, SyncOptionVideosMetadata),
		downloadsUpdates: vangogh_integration.FlagFromUrl(u, SyncOptionDownloadsUpdates),
	}

	if vangogh_integration.FlagFromUrl(u, "all") {
		so.items = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionItems))
		so.images = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionImages))
		so.screenshots = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionScreenshots))
		so.videosMetadata = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionVideosMetadata))
		so.downloadsUpdates = !vangogh_integration.FlagFromUrl(u, NegOpt(SyncOptionDownloadsUpdates))
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

	return Sync(
		since,
		syncOpts,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty),
		vangogh_integration.DownloadTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "no-patches"),
		vangogh_integration.DownloadsLayoutFromUrl(u),
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

	syncEvents := make(map[string][]string, 2)
	syncEvents[vangogh_integration.SyncStartKey] = []string{strconv.Itoa(int(syncStart))}

	if err := GetData(nil, nil, since, force); err != nil {
		return err
	}

	// summarize sync updates now, since other updates are digital artifacts
	// and won't affect the summaries
	if err := Summarize(syncStart); err != nil {
		return err
	}

	// get items (embedded into descriptions)
	if syncOpts.items {
		if err := GetDescriptionImages(nil, since, force); err != nil {
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
		if err := GetImages(nil, imageTypes, true, force); err != nil {
			return err
		}

		dehydratedImageTypes := []vangogh_integration.ImageType{vangogh_integration.Image, vangogh_integration.VerticalImage}
		if err := Dehydrate(nil, dehydratedImageTypes, false); err != nil {
			return err
		}
	}

	if syncOpts.videosMetadata {
		if err := GetVideoMetadata(nil, true, false); err != nil {
			return err
		}
	}

	// get downloads updates
	if syncOpts.downloadsUpdates {

		updatedDetails, err := shared_data.GetDetailsUpdates(since)
		if err != nil {
			return err
		}

		ids := slices.Collect(maps.Keys(updatedDetails))

		if err = UpdateDownloads(
			ids,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches,
			downloadsLayout,
			since,
			false); err != nil {
			return err
		}

		if err = validateUpdated(
			ids,
			since,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches); err != nil {
			return err
		}
	}

	if err := CacheGitHubReleases(force); err != nil {
		return err
	}

	syncEventsRdx, err := vangogh_integration.NewReduxWriter(vangogh_integration.SyncEventsProperty)
	if err != nil {
		return err
	}

	syncEvents[vangogh_integration.SyncCompleteKey] = []string{strconv.Itoa(int(time.Now().Unix()))}

	if err := syncEventsRdx.BatchReplaceValues(
		vangogh_integration.SyncEventsProperty,
		syncEvents); err != nil {
		return err
	}

	// backing up data
	if err := Backup(); err != nil {
		return err
	}

	// print new, updated
	return GetSummary()
}
