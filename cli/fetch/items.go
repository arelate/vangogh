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
	"sync"
	"time"
)

const maxConReq = 4

const defaultErrorDurationDays = 7

var errorsDurationDays = map[vangogh_integration.ProductType]int{
	// gog
	vangogh_integration.CatalogPage: 0,
	vangogh_integration.OrderPage:   0,
	vangogh_integration.AccountPage: 0,
	vangogh_integration.Details:     1,
}

type itemFetcher struct {
	itemReq   *reqs.Params
	kv        kevlar.KeyValues
	errs      map[string]error
	ch        chan string
	tpw       nod.TotalProgressWriter
	rateLimit float64
	mtx       *sync.Mutex
}

func (ife *itemFetcher) getNextItem(wg *sync.WaitGroup) {

	defer wg.Done()

	time.Sleep(time.Second * time.Duration(ife.rateLimit))

	if ife.tpw != nil {
		defer ife.tpw.Increment()
	}

	id := <-ife.ch

	productUrl := ife.itemReq.UrlFunc(id)
	if err := SetValue(id, productUrl, ife.itemReq, ife.kv); err != nil {
		ife.mtx.Lock()
		ife.errs[id] = err
		ife.mtx.Unlock()
	}
}

func Items(ids iter.Seq[string], itemReq *reqs.Params, kv kevlar.KeyValues, tpw nod.TotalProgressWriter) error {

	wg := new(sync.WaitGroup)
	ch := make(chan string, maxConReq)

	gif := &itemFetcher{
		itemReq: itemReq,
		kv:      kv,
		errs:    make(map[string]error),
		ch:      ch,
		tpw:     tpw,
		mtx:     new(sync.Mutex),
	}

	if itemReq.RateLimitRequests > 0 {
		gif.rateLimit = maxConReq * itemReq.RateLimitSeconds / itemReq.RateLimitRequests
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

	for id := range ids {

		if ok, skipErr := shouldSkipError(id, gif.itemReq.ProductType, rdx); skipErr == nil && ok {
			if tpw != nil {
				tpw.Increment()
			}
			continue
		} else if skipErr != nil {
			return skipErr
		}

		ch <- id
		wg.Add(1)
		go gif.getNextItem(wg)
	}

	wg.Wait()

	return shared_data.WriteTypeErrors(gif.errs, rdx)
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
