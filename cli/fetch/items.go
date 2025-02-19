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

var errorsDurationDays = map[vangogh_integration.ProductType]int{
	vangogh_integration.CatalogPage: 0,
	vangogh_integration.OrderPage:   0,
	vangogh_integration.AccountPage: 0,
}

type itemFetcher struct {
	itemReq   *reqs.Builder
	kv        kevlar.KeyValues
	errs      map[string]error
	ch        chan string
	tpw       nod.TotalProgressWriter
	rateLimit int
	mtx       *sync.Mutex
	rdx       redux.Writeable
}

func (ife *itemFetcher) getNextItem(wg *sync.WaitGroup) {

	defer wg.Done()
	if ife.tpw != nil {
		defer ife.tpw.Increment()
	}

	time.Sleep(time.Second * time.Duration(ife.rateLimit))

	id := <-ife.ch

	productUrl := ife.itemReq.UrlFunc(id)
	if err := SetValue(id, productUrl, ife.itemReq, ife.kv); err != nil {
		ife.mtx.Lock()
		ife.errs[id] = err
		ife.mtx.Unlock()
	}
}

func Items(ids iter.Seq[string], itemReq *reqs.Builder, kv kevlar.KeyValues, tpw nod.TotalProgressWriter) error {

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

	perTypeReduxDir, err := pathways.GetAbsRelDir(vangogh_integration.PerTypeRedux)
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

			errorExpires := dt.Add(time.Duration(errorsDurationDays[productType]) * 24 * time.Hour)

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
