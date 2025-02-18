package fetch

import (
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"iter"
	"sync"
	"time"
)

const maxConReq = 4

const (
	rateLimitRequests = 200
	rateLimitDuration = 5 * time.Minute
)

type itemFetcher struct {
	itemReq   *reqs.Builder
	kv        kevlar.KeyValues
	errs      map[string]error
	ch        chan string
	tpw       nod.TotalProgressWriter
	rateLimit time.Duration
	mtx       *sync.Mutex
}

func (ife *itemFetcher) getNextItem(wg *sync.WaitGroup) {

	defer wg.Done()
	if ife.tpw != nil {
		defer ife.tpw.Increment()
	}

	time.Sleep(ife.rateLimit)

	id := <-ife.ch

	productUrl := ife.itemReq.UrlFunc(id)
	if err := SetValue(id, productUrl, ife.itemReq, ife.kv); err != nil {
		ife.mtx.Lock()
		ife.errs[id] = err
		ife.mtx.Unlock()
	}
}

func Items(ids iter.Seq[string], itemReq *reqs.Builder, kv kevlar.KeyValues, tpw nod.TotalProgressWriter) map[string]error {

	wg := new(sync.WaitGroup)
	ch := make(chan string, maxConReq)

	gif := &itemFetcher{
		itemReq:   itemReq,
		kv:        kv,
		errs:      make(map[string]error),
		ch:        ch,
		tpw:       tpw,
		rateLimit: maxConReq * rateLimitDuration / rateLimitRequests,
		mtx:       new(sync.Mutex),
	}

	for id := range ids {
		ch <- id
		wg.Add(1)
		go gif.getNextItem(wg)
	}

	wg.Wait()

	return gif.errs
}
