package compton_fragments

import (
	"slices"
	"strconv"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func SyncStatus(r compton.Registrar, rdx redux.Readable, permissions ...author.Permission) compton.Element {

	var lastCompletedSyncEvent string
	var syncEventTimestamp int64

	for _, se := range vangogh_integration.SyncEventsSequence {
		if sss, ok := rdx.GetLastVal(vangogh_integration.SyncEventsProperty, se); ok {
			if sci, err := strconv.ParseInt(sss, 10, 64); err == nil {
				if sci >= syncEventTimestamp {
					lastCompletedSyncEvent = se
					syncEventTimestamp = sci
				}
			}
		}
	}

	nextSyncEvent := vangogh_integration.NextSyncEvent(lastCompletedSyncEvent)

	syncEventDateText := "Never"
	if syncEventTimestamp > 0 {
		syncEventDateText = time.Unix(syncEventTimestamp, 0).Format(time.DateTime)
	}

	var syncStatusColor color.Color
	var syncStatusSymbol compton.Symbol
	switch lastCompletedSyncEvent {
	case vangogh_integration.SyncCompleteKey:
		syncStatusColor = color.Green
		syncStatusSymbol = compton.CirclePositive
	case vangogh_integration.SyncInterruptedKey:
		syncStatusColor = color.Red
		syncStatusSymbol = compton.CrossNegative
	default:
		syncStatusColor = color.Yellow
		syncStatusSymbol = compton.TriangleNeutral
	}

	syncStatusFrow := compton.Frow(r).FontSize(size.XXSmall)

	syncStatusFrow.IconColor(syncStatusSymbol, syncStatusColor)

	switch nextSyncEvent {
	case vangogh_integration.SyncDownloadsKey:
		syncStatusFrow.LinkVal(vangogh_integration.SyncEventsTitles[nextSyncEvent], "/downloads-queue", syncEventDateText)
	default:
		syncStatusFrow.PropVal(vangogh_integration.SyncEventsTitles[nextSyncEvent], syncEventDateText)
	}

	if slices.Contains(permissions, perm.ReadLogs) {
		syncStatusFrow.LinkColor("Logs", "/logs", color.Foreground)
	}

	return compton.FICenter(r, syncStatusFrow).ColumnGap(size.Small)
}
