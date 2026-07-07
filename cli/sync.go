package cli

import (
	"errors"
	"maps"
	"net/url"
	"slices"
	"strconv"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/redux"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func SyncHandler(u *url.URL) error {
	since, err := vangogh_integration.SinceHouseAgoFromUrl(u)
	if err != nil {
		return err
	}

	q := u.Query()

	return Sync(
		since,
		vangogh_integration.OperatingSystemsFromUrl(u),
		vangogh_integration.LanguageCodesFromUrl(u),
		q.Has(vangogh_integration.UrlNoDlcsParameter),
		q.Has(vangogh_integration.UrlNoExtrasParameter),
		q.Has(vangogh_integration.UrlNoPatchesParameter),
		vangogh_integration.DownloadsLayoutFromUrl(u),
		q.Has(vangogh_integration.UrlAllDataParameter),
		q.Has(vangogh_integration.UrlForceParameter))
}

func Sync(
	since int64,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noDlcs, noExtras, noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	allData bool,
	force bool) error {

	// sync sequence:
	// 1.

	sa := nod.Begin("syncing data...")
	defer sa.Done()

	syncEventsRdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.VangoghSyncEventsProperty)
	if err != nil {
		return err
	}

	// setting since to last sync completed to be able to capture DLCs
	// purchased between syncs
	if since, err = setSinceToLastSyncCompleted(since, syncEventsRdx); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	syncStart := since
	if syncStart == 0 {
		syncStart = time.Now().Unix()
	}

	if err = setSyncEvent(vangogh_integration.SyncStartKey, syncEventsRdx); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	if err = GetData(nil, nil, since, new(dataFilter{purchases: true}), force); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncPurchasesDataKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// summarize sync updates now, since other updates are digital artifacts
	// and won't affect the summaries
	if err = Summarize(syncStart); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	// get images
	if err = GetImages(nil, gog_integration.AllImageTypes(), true, force); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncPurchasesImagesKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// get downloads updates

	var updatedDetails map[string]any
	updatedDetails, err = shared_data.GetGogDetailsUpdates(since)
	if err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	if err = queueDownloads(maps.Keys(updatedDetails)); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	if err = GetDownloads(nil,
		operatingSystems,
		langCodes,
		noDlcs,
		noExtras,
		noPatches,
		downloadsLayout,
		&getDownloadOptions{
			validate: true,
			queued:   true,
			force:    true,
		}); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncDownloadsKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	if err = GenerateMissingChecksums(operatingSystems, langCodes, noDlcs, noExtras, noPatches, downloadsLayout); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncGenerateMissingChecksums, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	if err = Cleanup(nil,
		operatingSystems,
		langCodes,
		noDlcs,
		noExtras,
		noPatches,
		downloadsLayout,
		true,
		false); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncCleanupKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	if err = GetVideoMetadata(nil, true, false); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncVideoMetadataKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	if err = GetBinaries(operatingSystems, true, true, force); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncBinaries, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// getting additional data
	if allData {

		if err = GetData(nil, nil, since, new(dataFilter{extraData: true, relatedApiProducts: true}), force); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		} else {
			if err = setSyncEvent(vangogh_integration.SyncExtraData, syncEventsRdx); err != nil {
				return setSyncInterrupted(err, syncEventsRdx)
			}
		}

		// summarize extra data updates now, since other updates are digital artifacts
		// and won't affect the summaries
		if err = Summarize(syncStart); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}

		// get extra images
		if err = GetImages(nil, gog_integration.AllImageTypes(), true, force); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		} else {
			if err = setSyncEvent(vangogh_integration.SyncExtraImagesKey, syncEventsRdx); err != nil {
				return setSyncInterrupted(err, syncEventsRdx)
			}
		}

		// get description images
		if err = GetDescriptionImages(nil, since, false, force); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		} else {
			if err = setSyncEvent(vangogh_integration.SyncDescriptionImagesKey, syncEventsRdx); err != nil {
				return setSyncInterrupted(err, syncEventsRdx)
			}
		}

	}

	// backing up data
	if err = Backup(); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncBackup, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// print new, updated
	if err = GetSummary(); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	if err = setSyncEvent(vangogh_integration.SyncCompleteKey, syncEventsRdx); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	}

	return nil
}

func setSyncEvent(eventKey string, rdx redux.Writeable) error {

	if err := rdx.MustHave(vangogh_integration.VangoghSyncEventsProperty); err != nil {
		return err
	}

	if !slices.Contains(vangogh_integration.SyncEventsSequence, eventKey) {
		return errors.New("unknown sync event key: " + eventKey)
	}

	syncEvents := make(map[string][]string)
	syncEvents[eventKey] = []string{strconv.FormatInt(time.Now().UTC().Unix(), 10)}

	return rdx.BatchReplaceValues(vangogh_integration.VangoghSyncEventsProperty, syncEvents)
}

func setSyncInterrupted(err error, rdx redux.Writeable) error {
	if sse := setSyncEvent(vangogh_integration.SyncInterruptedKey, rdx); sse != nil {
		return errors.Join(sse, err)
	}

	return err
}

func setSinceToLastSyncCompleted(since int64, syncEventsRdx redux.Readable) (int64, error) {

	utcNow := time.Now().UTC().Unix()
	// since-hours-ago specifies hours,
	// so anything less than an hour in seconds would be default value
	if utcNow-since < 60*60 {
		if lscs, ok := syncEventsRdx.GetLastVal(vangogh_integration.VangoghSyncEventsProperty, vangogh_integration.SyncCompleteKey); ok && lscs != "" {
			if lsci, err := strconv.ParseInt(lscs, 10, 64); err == nil {
				since = lsci
			} else {
				return -1, err
			}
		}
	}

	return since, nil
}
