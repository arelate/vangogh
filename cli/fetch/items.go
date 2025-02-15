package fetch

import (
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
	"sync"
	"time"
)

const maxConReq = 4

const (
	rateLimitRequests = 200
	rateLimitDuration = 5 * time.Minute
)

type IdUrlFunc func(id string) *url.URL

type itemFetcher struct {
	idUrlFunc       IdUrlFunc
	hc              *http.Client
	method          string
	userAccessToken string
	kv              kevlar.KeyValues
	errs            map[string]error
	ch              chan string
	tpw             nod.TotalProgressWriter
	rateLimit       time.Duration
	mtx             *sync.Mutex
}

func (ife *itemFetcher) getNextItem(wg *sync.WaitGroup) {

	defer wg.Done()
	if ife.tpw != nil {
		defer ife.tpw.Increment()
	}

	time.Sleep(ife.rateLimit)

	id := <-ife.ch

	productUrl := ife.idUrlFunc(id)
	if err := SetValue(id, productUrl, ife.hc, ife.method, ife.userAccessToken, ife.kv); err != nil {
		ife.mtx.Lock()
		ife.errs[id] = err
		ife.mtx.Unlock()
	}
}

func Items(idUrlFunc IdUrlFunc, hc *http.Client, method string, authBearer string, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, ids ...string) map[string]error {

	wg := new(sync.WaitGroup)
	ch := make(chan string, maxConReq)

	gif := &itemFetcher{
		idUrlFunc:       idUrlFunc,
		hc:              hc,
		method:          method,
		userAccessToken: authBearer,
		kv:              kv,
		errs:            make(map[string]error),
		ch:              ch,
		tpw:             tpw,
		rateLimit:       maxConReq * rateLimitDuration / rateLimitRequests,
		mtx:             new(sync.Mutex),
	}

	if tpw != nil {
		tpw.TotalInt(len(ids))
	}

	for _, id := range ids {
		ch <- id
		wg.Add(1)
		go gif.getNextItem(wg)
	}

	wg.Wait()

	return gif.errs
}
