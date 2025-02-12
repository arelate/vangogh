package fetchers

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/kevlar_dolo"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
	"strconv"
)

func NewIndexSetter(pt vangogh_integration.ProductType, ids []string) (dolo.IndexSetter, error) {

	localDir, err := vangogh_integration.AbsProductTypeDir(pt)
	if err != nil {
		return nil, err
	}

	valueSet, err := kevlar.New(localDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	return kevlar_dolo.NewIndexSetter(valueSet, ids...), nil
}

// Pages fetches all paged product type pages concurrently (using dolo.GetSet).
// To do that it downloads the first page, decodes that to get TotalPages,
// then constructs a slice of URLs and page ids to download all the remaining
// pages from 2nd to TotalPages using index setter.
func Pages(pt vangogh_integration.ProductType, since int64, httpClient *http.Client, tpw nod.TotalProgressWriter) error {

	gfp := nod.Begin(" getting the first %s...", pt)
	defer gfp.EndWithResult("done")

	up, err := vangogh_integration.NewUrlProvider(pt, nil)
	if err != nil {
		return err
	}

	//we need to handle the first page of the paged product type get-data request
	//separately from the rest, because:
	//1) initially we don't know how many pages paged data source would have (at least one is guaranteed)
	//2) first page contains the amount of pages data source has, so upon saving that we'll read it back
	//3) after figuring out how many pages data source has, we can construct URLs and page ids to get
	//the remaining set using dolo.GetSet and kevlar_dolo.IndexSetter.
	//Here is how we put that plan in motion:

	firstPage := "1"

	//construct a list of a single first page URL and page id "1"
	urls, ids := make([]*url.URL, 1), make([]string, 1)
	urls[0], ids[0] = up.Url(firstPage), firstPage

	//initiate IndexSetter using single page id "1"
	kis, err := NewIndexSetter(pt, ids)
	if err != nil {
		return err
	}

	dc := dolo.NewClient(httpClient, dolo.Defaults())

	//get the first page payload and set it in kevlar
	if errs := dc.GetSet(urls, kis, tpw, false); len(errs) > 0 {
		for ui, e := range errs {
			gfp.Error(fmt.Errorf("GetSet %s error: %s", urls[ui], e.Error()))
		}
		return fmt.Errorf("could not get the first page")
	}

	// certain data types increase monotonically and don't need to be downloaded completely (e.g. orders, wishlist)
	// if the first page has not been modified
	iua, err := kis.IsUpdatedAfter(0, since)
	if err != nil {
		return err
	}

	if vangogh_integration.IsFastPageFetchProduct(pt) && !iua {
		gfp.EndWithResult("first page unchanged, skipping the rest")
		return nil
	}

	//get downloaded first page from kevlar...
	fpReadCloser, err := kis.Get(0)
	defer fpReadCloser.Close()
	if err != nil {
		return err
	}

	//...and decode it using minimal data structure to get total pages amount
	var tpp gog_integration.TotalPagesProxy
	if err = json.NewDecoder(fpReadCloser).Decode(&tpp); err != nil {
		return err
	}

	//now that we know how many pages we have in total - reinitialize URLs and ids to
	//that number minus the first page we already got
	urls, ids = make([]*url.URL, tpp.GetTotalPages()-1), make([]string, tpp.GetTotalPages()-1)

	for i := 2; i <= tpp.GetTotalPages(); i++ {
		page := strconv.Itoa(i)
		//i-2 = first page (index: 0) will be 2
		urls[i-2] = up.Url(page)
		ids[i-2] = page
	}

	kis, err = NewIndexSetter(pt, ids)
	if err != nil {
		return err
	}

	if errs := dc.GetSet(urls, kis, tpw, false); len(errs) > 0 {
		for ui, e := range errs {
			tpw.Error(fmt.Errorf("GetSet %s error: %s", urls[ui], e.Error()))
		}
	}

	tpw.EndWithResult("done")

	return nil
}
