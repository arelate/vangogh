package fetch

import (
	"iter"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func Items(ids iter.Seq[string], itemReq *reqs.Params, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, force bool) error {

	var rateLimitMilliseconds int
	if itemReq.RateLimitRequests > 0 {
		rateLimitMilliseconds = itemReq.RateLimitMilliseconds / itemReq.RateLimitRequests
	}

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	itemsErrMessages := make(map[string][]string)
	itemsErrDates := make(map[string][]string)
	itemsLastUpdated := make(map[string][]string)

	for id := range ids {

		ptId := vangogh_integration.ProductTypeId(itemReq.ProductType, id)

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

	if err = rdx.BatchReplaceValues(vangogh_integration.VangoghGetDataLastUpdatedProperty, itemsLastUpdated); err != nil {
		return err
	}
	if err = rdx.BatchReplaceValues(vangogh_integration.VangoghGetDataErrorMessageProperty, itemsErrMessages); err != nil {
		return err
	}
	if err = rdx.BatchReplaceValues(vangogh_integration.VangoghGetDataErrorDateProperty, itemsErrDates); err != nil {
		return err
	}

	return nil
}
