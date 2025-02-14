package gog_data

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
	// https://support.gog.com/hc/en-us/articles/4405004689297-How-to-join-the-GOG-Affiliate-Program?product=gog
	// "Note that there is a 200 request/hour/IP on the api.gog.com/products/* endpoint"
	// vangogh uses other endpoint, and it seems like some of them have similar limits
	rateLimitRequests = 200
	rateLimitDuration = 5 * time.Minute
)

type idUrlFunc func(id string) *url.URL

type gogItemFetcher struct {
	productUrlFunc  idUrlFunc
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

func (gif *gogItemFetcher) getNextItem(wg *sync.WaitGroup) {

	defer wg.Done()
	if gif.tpw != nil {
		defer gif.tpw.Increment()
	}

	time.Sleep(gif.rateLimit)

	id := <-gif.ch

	productUrl := gif.productUrlFunc(id)
	if err := fetchGogData(id, productUrl, gif.hc, gif.method, gif.userAccessToken, gif.kv); err != nil {
		gif.mtx.Lock()
		gif.errs[id] = err
		gif.mtx.Unlock()
	}
}

func fetchGogItems(productUrlFunc idUrlFunc, hc *http.Client, method string, authBearer string, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, ids ...string) map[string]error {

	wg := new(sync.WaitGroup)
	ch := make(chan string, maxConReq)

	gif := &gogItemFetcher{
		productUrlFunc:  productUrlFunc,
		hc:              hc,
		method:          method,
		userAccessToken: authBearer,
		kv:              kv,
		errs:            make(map[string]error),
		ch:              ch,
		tpw:             tpw,
		rateLimit:       rateLimitDuration / rateLimitRequests,
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
