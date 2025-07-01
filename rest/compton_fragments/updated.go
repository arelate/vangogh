package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"strconv"
	"time"
)

func Updated(r compton.Registrar, rdx redux.Readable) compton.Element {

	var syncEvent string
	var syncEventTime time.Time

	for _, se := range []string{vangogh_integration.SyncStartKey, vangogh_integration.SyncCompleteKey} {
		if sss, ok := rdx.GetLastVal(vangogh_integration.SyncEventsProperty, se); ok {
			if sci, err := strconv.ParseInt(sss, 10, 64); err == nil {
				set := time.Unix(sci, 0)
				if set.After(syncEventTime) {
					syncEvent = se
					syncEventTime = set
				}
			}
		}
	}

	syncEventFrow := compton.Frow(r).FontSize(size.XXSmall)
	syncEventFrow.PropVal(vangogh_integration.SyncEventsTitles[syncEvent], syncEventTime.Format(time.RFC1123))

	return compton.FICenter(r, syncEventFrow).FontSize(size.XXSmall).ColumnGap(size.Small)
}
