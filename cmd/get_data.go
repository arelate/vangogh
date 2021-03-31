package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_pages"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/vangogh/internal"
	"io"
	"log"
	"strconv"
)

func GetData(
	ids []string,
	denyIds []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	since int64,
	missing bool,
	updated bool,
	verbose bool) error {

	if !vangogh_products.Valid(pt) {
		log.Printf("%s is not a valid product type", pt)
		return nil
	}

	if !vangogh_products.SupportsMedia(pt, mt) {
		if verbose {
			log.Printf("%s doesn't support %s media", pt, mt)
		}
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	if vangogh_products.RequiresAuth(pt) {
		if verbose {
			log.Printf("%s requires authenticated session, checking if user is logged in...", pt)
		}

		li, err := gog_auth.LoggedIn(httpClient)
		if err != nil {
			return err
		}

		if !li {
			log.Fatalf("user is not logged in")
		}
	}

	srcUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return err
	}

	dstUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}

	if vangogh_products.Paginated(pt) {
		if err := vangogh_pages.GetAllPages(httpClient, pt, mt); err != nil {
			return err
		}
		return split(pt, mt, since)
	} else {
		for _, mpt := range vangogh_products.MainTypes(pt) {
			if missing {
				if err = fetchMissing(pt, mpt, mt, denyIds, srcUrl, dstUrl, verbose); err != nil {
					return err
				}
			}
			if updated {
				if err = fetchUpdated(since, pt, mpt, mt, srcUrl, dstUrl, verbose); err != nil {
					return err
				}
			}
		}

		if !missing && !updated {
			return fetchItems(ids, pt, mt, srcUrl, dstUrl, verbose)
		}
		return nil
	}
}

func fetchItem(
	id string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) (io.Reader, error) {

	log.Printf("get %s (%s) data %s", pt, mt, id)

	httpClient, err := internal.HttpClient()
	if err != nil {
		return nil, err
	}

	u := sourceUrl(id, mt)
	if verbose {
		log.Printf("source url %s", u)
	}

	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("unexpected status: %s", resp.Status)
	}

	vs, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return resp.Body, err
	}

	var b bytes.Buffer
	tr := io.TeeReader(resp.Body, &b)

	if verbose {
		log.Printf("set value for item %s at destination %s", id, destUrl)
	}
	if err = vs.Set(id, tr); err != nil {
		return &b, err
	}

	if err := resp.Body.Close(); err != nil {
		return &b, err
	}

	return &b, nil
}

func fetchPages(
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) error {
	totalPages := 1
	for pp := 1; pp <= totalPages; pp++ {

		rdr, err := fetchItem(strconv.Itoa(pp), pt, mt, sourceUrl, destUrl, verbose)
		if err != nil {
			return err
		}

		var page gog_types.Page
		if err = json.NewDecoder(rdr).Decode(&page); err != nil {
			return err
		}

		totalPages = page.TotalPages
	}

	return nil
}

func fetchUpdated(
	since int64,
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	detailDestUrl string,
	verbose bool) error {
	log.Printf("get updated %s (%s) for %s", detailPt, mt, mainPt)

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return err
	}

	updatedIds := kvMain.ModifiedAfter(since)

	if len(updatedIds) > 0 {
		if verbose {
			log.Printf("updated %s ids: %v", detailPt, updatedIds)
		}
		if err := fetchItems(updatedIds, detailPt, mt, sourceUrl, detailDestUrl, verbose); err != nil {
			log.Println(err)
		}
	} else {
		if verbose {
			log.Printf("no updated %s data for %s (%s)\n", detailPt, mainPt, mt)
		}
	}

	return nil
}

func fetchMissing(
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	denyIds []string,
	sourceUrl vangogh_urls.ProductTypeUrl,
	detailDestUrl string,
	verbose bool) error {
	log.Printf("get missing %s (%s) for %s", detailPt, mt, mainPt)

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return err
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
	if err != nil {
		return err
	}
	missingIds := make([]string, 0)
	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) &&
			!internal.StringsContain(denyIds, id) {
			missingIds = append(missingIds, id)
		}
	}

	if len(missingIds) > 0 {
		if verbose {
			log.Printf("missing %s ids: %v", detailPt, missingIds)
		}
		if err := fetchItems(missingIds, detailPt, mt, sourceUrl, detailDestUrl, verbose); err != nil {
			log.Println(err)
		}
	} else {
		if verbose {
			log.Printf("no missing %s data for %s (%s)\n", detailPt, mainPt, mt)
		}
	}

	return nil
}

func fetchItems(
	ids []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) error {

	if !vangogh_products.SupportsFetch(pt) {
		return fmt.Errorf("getting %s data is not supported", pt)
	}

	if verbose {
		log.Printf("get data for ids: %v", ids)
	}

	for _, id := range ids {
		_, err := fetchItem(id, pt, mt, sourceUrl, destUrl, verbose)
		if err != nil {
			log.Printf("couldn't get data for %s (%s) %s: %v", pt, mt, id, err)
		}
	}
	return nil
}
