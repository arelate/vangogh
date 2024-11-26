package cli

import (
	"github.com/boggydigital/pathways"
	"net/url"
	"os"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/wits"
)

const (
	SyncOptionData             = "data"
	SyncOptionItems            = "items"
	SyncOptionImages           = "images"
	SyncOptionScreenshots      = "screenshots"
	SyncOptionVideosMetadata   = "videos-metadata"
	SyncOptionDownloadsUpdates = "downloads-Updates"
	negativePrefix             = "no-"
)

type syncOptions struct {
	data             bool
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
		data:             vangogh_local_data.FlagFromUrl(u, SyncOptionData),
		items:            vangogh_local_data.FlagFromUrl(u, SyncOptionItems),
		images:           vangogh_local_data.FlagFromUrl(u, SyncOptionImages),
		screenshots:      vangogh_local_data.FlagFromUrl(u, SyncOptionScreenshots),
		videosMetadata:   vangogh_local_data.FlagFromUrl(u, SyncOptionVideosMetadata),
		downloadsUpdates: vangogh_local_data.FlagFromUrl(u, SyncOptionDownloadsUpdates),
	}

	if vangogh_local_data.FlagFromUrl(u, "all") {
		so.data = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionData))
		so.items = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionItems))
		so.images = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionImages))
		so.screenshots = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionScreenshots))
		so.videosMetadata = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionVideosMetadata))
		so.downloadsUpdates = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionDownloadsUpdates))
	}

	return so
}

func SyncHandler(u *url.URL) error {
	syncOpts := initSyncOptions(u)

	since, err := vangogh_local_data.SinceFromUrl(u)
	if err != nil {
		return err
	}

	debug := vangogh_local_data.FlagFromUrl(u, "debug")

	return Sync(
		since,
		syncOpts,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "no-patches"),
		debug)
}

func Sync(
	since int64,
	syncOpts *syncOptions,
	operatingSystems []vangogh_local_data.OperatingSystem,
	langCodes []string,
	downloadTypes []vangogh_local_data.DownloadType,
	noPatches bool,
	debug bool) error {

	if debug {
		absLogsDir, err := pathways.GetAbsDir(vangogh_local_data.Logs)
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
	defer sa.End()

	syncStart := since
	if syncStart == 0 {
		syncStart = time.Now().Unix()
	}

	syncEvents := make(map[string][]string, 2)
	syncEvents[vangogh_local_data.SyncStartKey] = []string{strconv.Itoa(int(syncStart))}

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

	if syncOpts.data {

		//get GOG.com, Steam array and paged data
		pagedArrayData := append(
			vangogh_local_data.GOGArrayProducts(),
			vangogh_local_data.GOGPagedProducts()...)
		pagedArrayData = append(pagedArrayData,
			vangogh_local_data.SteamArrayProducts()...)
		pagedArrayData = append(pagedArrayData,
			vangogh_local_data.HLTBArrayProducts()...)

		for _, pt := range pagedArrayData {
			if err := GetData(nil, nil, pt, since, false, false); err != nil {
				return sa.EndWithError(err)
			}
		}

		detailData := vangogh_local_data.GOGDetailProducts()
		detailData = append(detailData, vangogh_local_data.PCGWPageId)

		//get GOG.com detail data, PCGamingWiki pageId, steamAppId
		if err := getDetailData(detailData, since); err != nil {
			return sa.EndWithError(err)
		}

		//reduce PCGamingWiki pageId
		if err := Reduce(since, []string{vangogh_local_data.PCGWPageIdProperty}, true); err != nil {
			return sa.EndWithError(err)
		}

		//get PCGamingWiki externallinks, engine
		//this needs to happen after reduce, since PCGW PageId - GOG.com ProductId
		//connection is established at reduce from cargo data.
		pcgwDetailProducts := []vangogh_local_data.ProductType{
			vangogh_local_data.PCGWEngine,
			vangogh_local_data.PCGWExternalLinks,
		}

		if err := getDetailData(pcgwDetailProducts, since); err != nil {
			return sa.EndWithError(err)
		}

		//reduce SteamAppId, HowLongToBeatId, IGDBId
		if err := Reduce(since, []string{
			vangogh_local_data.SteamAppIdProperty,
			vangogh_local_data.HLTBBuildIdProperty,
			vangogh_local_data.HLTBIdProperty,
			vangogh_local_data.IGDBIdProperty}, true); err != nil {
			return sa.EndWithError(err)
		}

		otherDetailProducts := vangogh_local_data.SteamDetailProducts()
		otherDetailProducts = append(otherDetailProducts, vangogh_local_data.HLTBDetailProducts()...)

		//get other detail products (Steam data, HLTB data)
		//this needs to happen after reduce, since Steam AppId - GOG.com ProductId
		//connection is established at reduce. And the earlier data set cannot be retrieved post reduce,
		//since SteamAppList is fetched with initial data
		if err := getDetailData(otherDetailProducts, since); err != nil {
			return sa.EndWithError(err)
		}

		// finally, reduce all properties
		if err := Reduce(since, vangogh_local_data.ReduxProperties(), false); err != nil {
			return sa.EndWithError(err)
		}
	}

	// summarize sync updates now, since other updates are digital artifacts
	// and won't affect the summaries
	if err := Summarize(syncStart); err != nil {
		return sa.EndWithError(err)
	}

	// get items (embedded into descriptions)
	if syncOpts.items {
		if err := GetItems(nil, since); err != nil {
			return sa.EndWithError(err)
		}
	}

	// get images
	if syncOpts.images {
		imageTypes := make([]vangogh_local_data.ImageType, 0, len(vangogh_local_data.AllImageTypes()))
		for _, it := range vangogh_local_data.AllImageTypes() {
			if !syncOpts.screenshots && it == vangogh_local_data.Screenshots {
				continue
			}
			imageTypes = append(imageTypes, it)
		}
		if err := GetImages(nil, imageTypes, true); err != nil {
			return sa.EndWithError(err)
		}

		if err := Dehydrate(nil, vangogh_local_data.ImageTypesDehydration(), false); err != nil {
			return sa.EndWithError(err)
		}
	}

	if syncOpts.videosMetadata {
		if err := GetVideoMetadata(nil, true, false); err != nil {
			return sa.EndWithError(err)
		}
	}

	// get downloads updates
	if syncOpts.downloadsUpdates {
		if err := UpdateDownloads(
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches,
			since,
			false); err != nil {
			return sa.EndWithError(err)
		}

		if err := validateUpdated(
			since,
			operatingSystems,
			langCodes,
			downloadTypes,
			noPatches); err != nil {
			return sa.EndWithError(err)
		}

		if err := CascadeValidation(operatingSystems, langCodes, downloadTypes, noPatches); err != nil {
			return sa.EndWithError(err)
		}
	}

	syncEventsRdx, err := vangogh_local_data.NewReduxWriter(vangogh_local_data.SyncEventsProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	syncEvents[vangogh_local_data.SyncCompleteKey] = []string{strconv.Itoa(int(time.Now().Unix()))}

	if err := syncEventsRdx.BatchReplaceValues(
		vangogh_local_data.SyncEventsProperty,
		syncEvents); err != nil {
		return sa.EndWithError(err)
	}

	// backing up data
	if err := Backup(); err != nil {
		return sa.EndWithError(err)
	}

	sa.EndWithResult("done")

	// print new, updated
	return GetSummary()
}

func getDetailData(pts []vangogh_local_data.ProductType, since int64) error {
	for _, pt := range pts {

		var skipList wits.KeyValues

		aslp, err := vangogh_local_data.AbsSkipListPath()
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
		if err := GetData(nil, skipIds, pt, since, true, true); err != nil {
			return err
		}
	}

	return nil
}
