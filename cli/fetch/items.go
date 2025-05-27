package fetch

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"time"
)

func Items(ids iter.Seq[string], itemReq *reqs.Params, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, force bool) error {

	var rateLimitMilliseconds int
	if itemReq.RateLimitRequests > 0 {
		rateLimitMilliseconds = itemReq.RateLimitMilliseconds / itemReq.RateLimitRequests
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	itemsErrMessages := make(map[string][]string)
	itemsErrDates := make(map[string][]string)
	itemsLastUpdated := make(map[string][]string)

	for id := range ids {

		var ptId string
		ptId, err = vangogh_integration.ProductTypeId(itemReq.ProductType, id)
		if err != nil {
			return err
		}

		itemsErrMessages[ptId] = nil
		itemsErrDates[ptId] = make([]string, 0)

		shouldSkip, skipErr := shared_data.ShouldSkip(id, itemReq.ProductType, rdx)
		if skipErr != nil {
			return skipErr
		}

		if shouldSkip && !force {
			if tpw != nil {
				tpw.Increment()
			}
			continue
		}

		if kv.Has(id) && !force {

			shouldUpdate, updErr := shared_data.ShouldUpdate(id, itemReq.ProductType, rdx)
			if updErr != nil {
				return updErr
			}

			if !shouldUpdate {
				if tpw != nil {
					tpw.Increment()
				}
				continue
			}
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)

		productUrl := itemReq.UrlFunc(id)
		if err = RequestSetValue(id, productUrl, itemReq, kv); err != nil {
			itemsErrMessages[ptId] = append(itemsErrMessages[ptId], err.Error())
			itemsErrDates[ptId] = append(itemsErrDates[ptId], formattedNow)
		}

		if len(itemsErrMessages[ptId]) == 0 {
			itemsLastUpdated[ptId] = append(itemsLastUpdated[ptId], formattedNow)
		}

		if rateLimitMilliseconds > 0 {
			time.Sleep(time.Duration(rateLimitMilliseconds) * time.Millisecond)
		}

		if tpw != nil {
			tpw.Increment()
		}
	}

	if err = rdx.BatchReplaceValues(vangogh_integration.GetDataLastUpdatedProperty, itemsLastUpdated); err != nil {
		return err
	}
	if err = rdx.BatchReplaceValues(vangogh_integration.GetDataErrorMessageProperty, itemsErrMessages); err != nil {
		return err
	}
	if err = rdx.BatchReplaceValues(vangogh_integration.GetDataErrorDateProperty, itemsErrDates); err != nil {
		return err
	}

	return nil
}
