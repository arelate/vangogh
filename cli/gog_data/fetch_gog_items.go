package gog_data

import (
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
	"sync"
)

const maxConReq = 4

type idUrlFunc func(id string) *url.URL

type gogItemFetcher struct {
	productUrlFunc idUrlFunc
	hc             *http.Client
	method         string
	kv             kevlar.KeyValues
	errs           map[string]error
	ch             chan string
	tpw            nod.TotalProgressWriter
	mtx            *sync.Mutex
}

func (gif *gogItemFetcher) getNextItem(wg *sync.WaitGroup) {

	defer wg.Done()
	if gif.tpw != nil {
		defer gif.tpw.Increment()
	}

	id := <-gif.ch

	productUrl := gif.productUrlFunc(id)
	if err := fetchGogData(id, productUrl, gif.hc, gif.method, gif.kv); err != nil {
		gif.mtx.Lock()
		gif.errs[id] = err
		gif.mtx.Unlock()
	}
}

func fetchGogItems(productUrlFunc idUrlFunc, hc *http.Client, method string, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, ids ...string) map[string]error {

	wg := new(sync.WaitGroup)
	ch := make(chan string, maxConReq)

	gif := &gogItemFetcher{
		productUrlFunc: productUrlFunc,
		hc:             hc,
		method:         method,
		kv:             kv,
		errs:           make(map[string]error),
		ch:             ch,
		tpw:            tpw,
		mtx:            new(sync.Mutex),
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
