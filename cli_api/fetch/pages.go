package fetch

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"io"
	"net/http"
	"net/url"
	"strconv"
)

//TODO: move this to kvas
type kvasIndexSetter struct {
	valueSet *kvas.ValueSet
	ids      []string
}

func (kis *kvasIndexSetter) Len() int {
	return len(kis.ids)
}

func (kis *kvasIndexSetter) Exists(int) bool {
	//kvas performs hash computation to track modified files,
	//so we want all set attempts to go through (we need to
	//read src to compute that hash)
	return false
}

func (kis *kvasIndexSetter) Set(index int, src io.ReadCloser, completion chan bool, errors chan error) {

	defer src.Close()

	if index < 0 || index >= len(kis.ids) {
		errors <- fmt.Errorf("id index out of bounds")
	}

	if err := kis.valueSet.Set(kis.ids[index], src); err != nil {
		errors <- err
	}

	completion <- true
}

func (kis *kvasIndexSetter) Get(key string) (io.ReadCloser, error) {
	return kis.valueSet.Get(key)
}

func NewKvasIndexSetter(pt vangogh_products.ProductType, mt gog_media.Media, ids []string) (*kvasIndexSetter, error) {

	localDir, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return nil, err
	}

	valueSet, err := kvas.NewJsonLocal(localDir)
	if err != nil {
		return nil, err
	}

	return &kvasIndexSetter{
		valueSet: valueSet,
		ids:      ids,
	}, nil
}

func Pages(pt vangogh_products.ProductType, mt gog_media.Media, httpClient *http.Client, tpw nod.TotalProgressWriter) error {

	gfp := nod.Begin(" getting first %s (%s) page...", pt, mt)
	defer gfp.End()

	remoteUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return err
	}

	firstPage := "1"

	urls, ids := make([]*url.URL, 1), make([]string, 1)
	urls[0], ids[0] = remoteUrl(firstPage, mt), firstPage

	kis, err := NewKvasIndexSetter(pt, mt, ids)
	if err != nil {
		return err
	}

	if err := dolo.GetSet(urls, kis, httpClient, tpw); err != nil {
		return err
	}

	readCloser, err := kis.Get(firstPage)
	defer readCloser.Close()
	if err != nil {
		return err
	}

	var page gog_types.TotalPages
	if err = json.NewDecoder(readCloser).Decode(&page); err != nil {
		return tpw.EndWithError(err)
	}

	gfp.EndWithResult("done")

	urls, ids = make([]*url.URL, page.TotalPages-1), make([]string, page.TotalPages-1)

	for i := 2; i <= page.TotalPages; i++ {
		page := strconv.Itoa(i)
		urls[i-2] = remoteUrl(page, mt)
		ids[i-2] = page
	}

	kis, err = NewKvasIndexSetter(pt, mt, ids)
	if err != nil {
		return err
	}

	if err := dolo.GetSet(urls, kis, httpClient, tpw); err != nil {
		return err
	}

	tpw.EndWithResult("done")

	return nil
}
