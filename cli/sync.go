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

	operatingSystems := vangogh_integration.OperatingSystemsFromUrl(u)
	languageCodes := vangogh_integration.LanguageCodesFromUrl(u)
	noDlc := q.Has(vangogh_integration.UrlNoDlcsParameter)
	noExtras := q.Has(vangogh_integration.UrlNoExtrasParameter)
	noPatches := q.Has(vangogh_integration.UrlNoPatchesParameter)
	downloadsLayout := vangogh_integration.DownloadsLayoutFromUrl(u)
	additionalData := q.Has(vangogh_integration.UrlAdditionalDataParameter)
	force := q.Has(vangogh_integration.UrlForceParameter)

	return Sync(since,
		operatingSystems,
		languageCodes,
		noDlc, noExtras, noPatches,
		downloadsLayout,
		additionalData,
		force)
}

func Sync(
	since int64,
	operatingSystems []vangogh_integration.OperatingSystem,
	langCodes []string,
	noDlcs, noExtras, noPatches bool,
	downloadsLayout vangogh_integration.DownloadsLayout,
	additionalData bool,
	force bool) error {

	// sync sequence:
	// 0. resolve since parameter and set to last sync completed if default
	// 1. get account data
	// 2. get missing images
	// 3. get downloads updates and validate them
	// 4. generate missing checksums
	// 5. cleanup - remove downloads not linked to account data anymore (previous installer versions)
	// 6. get videos metadata (titles)
	// 7. get binaries - WINE and SteamCMD
	// 8. get additional data and...
	// - 8.1. images for additional data
	// - 8.2. description images for additional data
	// 9. backup metadata

	sa := nod.Begin("syncing data...")
	defer sa.Done()

	syncEventsRdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.VangoghSyncEventsProperty)
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

	// 1
	if err = getAccountData(since, force); err != nil {
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

	// 2
	if err = GetImages(nil, gog_integration.AllImageTypes(), true, force); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncPurchasesImagesKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// 3
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
		new(getDownloadOptions{
			validate: true,
			queued:   true,
			force:    true,
		})); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncDownloadsKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// 4
	if err = GenerateMissingChecksums(operatingSystems, langCodes, noDlcs, noExtras, noPatches, downloadsLayout); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncGenerateMissingChecksums, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// 5
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

	// 6
	if err = GetVideoMetadata(nil, true, false); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncVideoMetadataKey, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// 7
	if err = GetBinaries(operatingSystems, true, true, force); err != nil {
		return setSyncInterrupted(err, syncEventsRdx)
	} else {
		if err = setSyncEvent(vangogh_integration.SyncBinaries, syncEventsRdx); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		}
	}

	// 8
	if additionalData {

		if err = getAdditionalData(since, force); err != nil {
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

		// 8.1
		if err = GetImages(nil, gog_integration.AllImageTypes(), true, force); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		} else {
			if err = setSyncEvent(vangogh_integration.SyncExtraImagesKey, syncEventsRdx); err != nil {
				return setSyncInterrupted(err, syncEventsRdx)
			}
		}

		// 8.2
		if err = GetDescriptionImages(nil, since, false, force); err != nil {
			return setSyncInterrupted(err, syncEventsRdx)
		} else {
			if err = setSyncEvent(vangogh_integration.SyncDescriptionImagesKey, syncEventsRdx); err != nil {
				return setSyncInterrupted(err, syncEventsRdx)
			}
		}

	}

	// 9
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
		if lscs, ok := syncEventsRdx.GetLastVal(
			vangogh_integration.VangoghSyncEventsProperty,
			vangogh_integration.SyncCompleteKey); ok && lscs != "" {
			if lsci, err := strconv.ParseInt(lscs, 10, 64); err == nil {
				since = lsci
			} else {
				return -1, err
			}
		}
	}

	return since, nil
}
