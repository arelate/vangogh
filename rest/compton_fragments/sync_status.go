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

	for _, se := range vangogh_integration.SyncEventsKeys {
		if sss, ok := rdx.GetLastVal(vangogh_integration.SyncEventsProperty, se); ok {
			if sci, err := strconv.ParseInt(sss, 10, 64); err == nil {
				if sci >= syncEventTimestamp {
					lastCompletedSyncEvent = se
					syncEventTimestamp = sci
				}
			}
		}
	}

	var currentSyncEvent string
	if cse, ok := vangogh_integration.CurrentSyncEventForCompleted[lastCompletedSyncEvent]; ok {
		currentSyncEvent = cse
	} else {
		currentSyncEvent = lastCompletedSyncEvent
	}

	syncEventDateText := "Never"
	if syncEventTimestamp > 0 {
		syncEventDateText = time.Unix(syncEventTimestamp, 0).Format("02-Jan-06 15:04")
	}

	var syncStatusColor color.Color
	switch lastCompletedSyncEvent {
	case vangogh_integration.SyncCompleteKey:
		syncStatusColor = color.Green
	case vangogh_integration.SyncInterruptedKey:
		syncStatusColor = color.Red
	default:
		syncStatusColor = color.Yellow
	}

	syncStatusFrow := compton.Frow(r).FontSize(size.XXSmall)

	syncStatusFrow.IconColor(compton.SmallerCircle, syncStatusColor)
	syncStatusFrow.PropVal(vangogh_integration.CurrentSyncEventsTitles[currentSyncEvent], syncEventDateText)

	if slices.Contains(permissions, perm.ReadLogs) {
		syncStatusFrow.LinkColor("Logs", "/logs", color.RepForeground)
	}

	return compton.FICenter(r, syncStatusFrow).ColumnGap(size.Small)
}
