package compton_fragments

import (
	"strconv"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

const syncStatusTitle = "Sync status"

func SyncStatus(r compton.Registrar, rdx redux.Readable) compton.Element {

	var lastCompletedSyncEvent, currentSyncEvent string
	var syncEventTimestamp int64

	for ii, se := range vangogh_integration.SyncEventsKeys {
		if sss, ok := rdx.GetLastVal(vangogh_integration.SyncEventsProperty, se); ok {
			if sci, err := strconv.ParseInt(sss, 10, 64); err == nil {
				if sci >= syncEventTimestamp {
					lastCompletedSyncEvent = se
					if ii < len(vangogh_integration.SyncEventsKeys)-1 {
						currentSyncEvent = vangogh_integration.SyncEventsKeys[ii+1]
					} else {
						currentSyncEvent = lastCompletedSyncEvent
					}
					syncEventTimestamp = sci
				}
			}
		}
	}

	syncEventDateText := "Never"
	if syncEventTimestamp > 0 {
		syncEventDateText = time.Unix(syncEventTimestamp, 0).Format(time.RFC1123)
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

	syncStatusFrow.Heading(syncStatusTitle)
	syncStatusFrow.IconColor(compton.SmallerCircle, syncStatusColor)
	syncStatusFrow.PropVal(vangogh_integration.CurrentSyncEventsTitles[currentSyncEvent], syncEventDateText)

	return compton.FICenter(r, syncStatusFrow).FontSize(size.XXSmall).ColumnGap(size.Small)
}
