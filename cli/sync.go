package cli

import (
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
	SyncOptionVideos           = "videos"
	SyncOptionThumbnails       = "thumbnails"
	SyncOptionDownloadsUpdates = "downloads-Updates"
	negativePrefix             = "no-"
)

type syncOptions struct {
	data             bool
	items            bool
	images           bool
	screenshots      bool
	videos           bool
	thumbnails       bool
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
		videos:           vangogh_local_data.FlagFromUrl(u, SyncOptionVideos),
		thumbnails:       vangogh_local_data.FlagFromUrl(u, SyncOptionThumbnails),
		downloadsUpdates: vangogh_local_data.FlagFromUrl(u, SyncOptionDownloadsUpdates),
	}

	if vangogh_local_data.FlagFromUrl(u, "all") {
		so.data = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionData))
		so.items = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionItems))
		so.images = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionImages))
		so.screenshots = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionScreenshots))
		so.videos = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionVideos))
		so.thumbnails = !vangogh_local_data.FlagFromUrl(u, NegOpt(SyncOptionThumbnails))
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

	return Sync(
		since,
		syncOpts,
		vangogh_local_data.OperatingSystemsFromUrl(u),
		vangogh_local_data.DownloadTypesFromUrl(u),
		vangogh_local_data.ValuesFromUrl(u, "language-code"))
}

func Sync(
	since int64,
	syncOpts *syncOptions,
	operatingSystems []vangogh_local_data.OperatingSystem,
	downloadTypes []vangogh_local_data.DownloadType,
	langCodes []string) error {

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
	//- Fetch GOG.com array and paged data
	//- Fetch Steam array data (required for SteamAppId reduction)
	//- Fetch PCGamingWiki Cargo data (required for SteamAppId and PCGWPageId reduction)
	//- Reduce Title, SteamAppId and PCGWPageId data
	//- Fetch Steam detail data (using SteamAppId data)
	//- Fetch PCGamingWiki Wikitext data (required for all other data id reductions)
	//- ... (will be used to reduce other data ids)
	//- ... (will be used to fetch other data types using data ids)
	//- Reduce all properties from the data

	if syncOpts.data {

		//get GOG.com array and paged data
		paData := append(vangogh_local_data.GOGArrayProducts(),
			vangogh_local_data.GOGPagedProducts()...)

		for _, pt := range paData {
			if err := GetData(map[string]bool{}, nil, pt, since, false, false); err != nil {
				return sa.EndWithError(err)
			}
		}

		//get Steam array data
		for _, pt := range vangogh_local_data.SteamArrayProducts() {
			if err := GetData(map[string]bool{}, nil, pt, since, false, false); err != nil {
				return sa.EndWithError(err)
			}
		}

		//get GOG.com main - detail data
		if err := getDetailData(vangogh_local_data.GOGDetailProducts(), since); err != nil {
			return sa.EndWithError(err)
		}

		//get PCGamingWiki cargo data
		if err := getDetailData([]vangogh_local_data.ProductType{vangogh_local_data.PCGWCargo}, since); err != nil {
			return sa.EndWithError(err)
		}

		//reduce Title and Steam AppId
		if err := Reduce(since, []string{
			vangogh_local_data.TitleProperty,
			vangogh_local_data.PCGWPageId,
			vangogh_local_data.SteamAppIdProperty}, true); err != nil {
			return sa.EndWithError(err)
		}

		//get Steam main - detail data
		//this needs to happen after reduce, since Steam AppId - GOG.com ProductId
		//connection is established at reduce. And the earlier data set cannot be retrieved post reduce,
		//since SteamAppList is fetched with initial data
		if err := getDetailData(vangogh_local_data.SteamDetailProducts(), since); err != nil {
			return sa.EndWithError(err)
		}

		//get PCGamingWiki cargo data
		//this needs to happen after reduce, since PCGW PageId - GOG.com ProductId
		//connection is established at reduce from cargo data.
		if err := getDetailData([]vangogh_local_data.ProductType{vangogh_local_data.PCGWWikiText}, since); err != nil {
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
		if err := GetItems(map[string]bool{}, since); err != nil {
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
		if err := GetImages(map[string]bool{}, imageTypes, true); err != nil {
			return sa.EndWithError(err)
		}
	}

	// get downloads updates
	if syncOpts.downloadsUpdates {
		if err := UpdateDownloads(
			operatingSystems,
			downloadTypes,
			langCodes,
			since,
			false); err != nil {
			return sa.EndWithError(err)
		}

		if err := validateUpdated(
			since,
			operatingSystems,
			downloadTypes,
			langCodes); err != nil {
			return sa.EndWithError(err)
		}
	}

	// get videos
	if syncOpts.videos {
		if err := GetVideos(map[string]bool{}, true, false); err != nil {
			return sa.EndWithError(err)
		}
	}

	// get thumbnails
	if syncOpts.thumbnails {
		if err := GetThumbnails(map[string]bool{}, true, false); err != nil {
			return sa.EndWithError(err)
		}
	}

	syncEventsRxa, err := vangogh_local_data.ConnectReduxAssets(vangogh_local_data.SyncEventsProperty)
	if err != nil {
		return sa.EndWithError(err)
	}

	syncEvents[vangogh_local_data.SyncCompleteKey] = []string{strconv.Itoa(int(time.Now().Unix()))}

	if err := syncEventsRxa.BatchReplaceValues(
		vangogh_local_data.SyncEventsProperty,
		syncEvents); err != nil {
		return sa.EndWithError(err)
	}

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

		if _, err := os.Stat(vangogh_local_data.AbsSkipListPath()); err == nil {
			slFile, err := os.Open(vangogh_local_data.AbsSkipListPath())
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
		if err := GetData(map[string]bool{}, skipIds, pt, since, true, true); err != nil {
			return err
		}
	}

	return nil
}
