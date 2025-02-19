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

		if skip, skipErr := shared_data.SkipError(id, itemReq.ProductType, rdx); skip && skipErr == nil {
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

		if rateLimit > 0 {
			time.Sleep(rateLimit)
		}

		if tpw != nil {
			tpw.Increment()
		}
	}

	return shared_data.WriteTypeErrors(errs, rdx)
}
