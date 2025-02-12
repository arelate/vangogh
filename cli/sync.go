package cli

import (
	"github.com/boggydigital/pathways"
	"net/url"
	"os"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/wits"
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
	defer sa.EndWithResult("done")

	syncStart := since
	if syncStart == 0 {
		syncStart = time.Now().Unix()
	}

	syncEvents := make(map[string][]string, 2)
	syncEvents[vangogh_integration.SyncStartKey] = []string{strconv.Itoa(int(syncStart))}

	//data sync is a sequence of specific data types that is required to get
	//all supported data types in one sync session (assuming the connection data
	//is available and the data itself if available, of course)
	//below is a current sequence:
	//
	//- get GOG.com, Steam array and paged data
	//- get GOG.com detail data, PCGamingWiki pageId, steamAppId
	//- reduce PCGamingWiki pageId
	//- get PCGamingWiki externallinks, engine
	//- reduce SteamAppId, HowLongToBeatId, IGDBId
	//- get other detail products (Steam data, HLTB data)
	//- finally, reduce all properties

	if err := GetData(since, force); err != nil {
		return err
	}

	// summarize sync updates now, since other updates are digital artifacts
	// and won't affect the summaries
	if err := Summarize(syncStart); err != nil {
		return err
	}

	// get items (embedded into descriptions)
	if syncOpts.items {
		if err := GetItems(nil, since); err != nil {
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
		if err := GetImages(nil, imageTypes, true); err != nil {
			return err
		}

		if err := Dehydrate(nil, vangogh_integration.ImageTypesDehydration(), false); err != nil {
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

		ids, err := itemizeUpdatedAccountProducts(since)
		if err != nil {
			return err
		}

		if err := UpdateDownloads(
			ids,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches,
			since,
			false); err != nil {
			return err
		}

		if err := validateUpdated(
			ids,
			since,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches); err != nil {
			return err
		}
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

func getDetailData(pts []vangogh_integration.ProductType, since int64) error {
	for _, pt := range pts {

		var skipList wits.KeyValues

		aslp, err := vangogh_integration.AbsSkipListPath()
		if err != nil {
			return err
		}

		if _, err := os.Stat(aslp); err == nil {
			slFile, err := os.Open(aslp)
			if err != nil {
				slFile.Close()
				return err
			}

			skipList, err = wits.ReadKeyValues(slFile)
			slFile.Close()
			if err != nil {
				return err
			}
		}

		skipIds := skipList[pt.String()]
		if err := GetDataLegacy(nil, skipIds, pt, since, true, true); err != nil {
			return err
		}
	}

	return nil
}
