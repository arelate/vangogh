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
	"path/filepath"
	"time"
)

const defaultErrorDurationDays = 7

var errorsDurationDays = map[vangogh_integration.ProductType]int{
	vangogh_integration.CatalogPage: 0, // GOG pages errors are exceptional and likely indicate
	vangogh_integration.OrderPage:   0,
	vangogh_integration.AccountPage: 0,
	vangogh_integration.Details:     1,
}

func Items(ids iter.Seq[string], itemReq *reqs.Params, kv kevlar.KeyValues, tpw nod.TotalProgressWriter) error {

	var rateLimit time.Duration
	if itemReq.RateLimitRequests > 0 {
		rateLimit = time.Duration(itemReq.RateLimitSeconds * float64(time.Second) / itemReq.RateLimitRequests)
	}

	perTypeReduxDir, err := pathways.GetAbsRelDir(vangogh_integration.TypeErrors)
	if err != nil {
		return err
	}

	typeReduxDir := filepath.Join(perTypeReduxDir, itemReq.ProductType.String())

	rdx, err := redux.NewWriter(typeReduxDir, vangogh_integration.TypeErrorProperties()...)
	if err != nil {
		return err
	}

	errs := make(map[string]error)

	for id := range ids {

		if ok, skipErr := shouldSkipError(id, itemReq.ProductType, rdx); skipErr == nil && ok {
			if tpw != nil {
				tpw.Increment()
			}
			continue
		} else if skipErr != nil {
			return skipErr
		}

		productUrl := itemReq.UrlFunc(id)
		if err = SetValue(id, productUrl, itemReq, kv); err != nil {
			errs[id] = err
		}

		time.Sleep(rateLimit)

		if tpw != nil {
			tpw.Increment()
		}
	}

	return shared_data.WriteTypeErrors(errs, rdx)
}

func shouldSkipError(id string, productType vangogh_integration.ProductType, rdx redux.Writeable) (bool, error) {

	if dateStr, ok := rdx.GetLastVal(vangogh_integration.TypeErrorDateProperty, id); ok && dateStr != "" {
		if dt, err := time.Parse(time.RFC3339, dateStr); err == nil {

			var errorDuration time.Duration
			if edd, sure := errorsDurationDays[productType]; sure {
				errorDuration = time.Duration(edd)
			} else {
				errorDuration = defaultErrorDurationDays
			}

			errorExpires := dt.Add(errorDuration * 24 * time.Hour)

			if time.Now().UTC().Before(errorExpires) {

				nod.Log("skipping current %s error last encountered: %s", productType, dateStr)

				return true, nil
			} else {

				nod.Log("clearing %s error last encountered: %s", productType, dateStr)

				if err = rdx.CutKeys(vangogh_integration.TypeErrorDateProperty, id); err != nil {
					return false, err
				}
				if err = rdx.CutKeys(vangogh_integration.TypeErrorMessageProperty, id); err != nil {
					return false, err
				}

				return false, nil
			}

		} else {
			return false, err
		}
	}

	return false, nil
}
