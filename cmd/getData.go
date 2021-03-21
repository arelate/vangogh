package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
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
	pt vangogh_types.ProductType,
	mt gog_media.Media,
	timestamp int64,
	missing bool,
	verbose bool) error {

	if !vangogh_types.ValidProductType(pt) {
		log.Printf("%s is not a valid product type", pt)
		return nil
	}

	if !vangogh_types.SupportsMedia(pt, mt) {
		if verbose {
			log.Printf("%s doesn't support %s media", pt, mt)
		}
		return nil
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	if vangogh_types.ProductTypeRequiresAuth(pt) {
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

	if vangogh_types.HasPages(pt) {
		if err := fetchPages(pt, mt, srcUrl, dstUrl, verbose); err != nil {
			return err
		}
		return split(pt, mt, timestamp)
	} else {
		if missing {
			for _, mpt := range vangogh_types.MainProductTypes(pt) {
				mainDstUrl, err := vangogh_urls.LocalProductsDir(mpt, mt)
				if err != nil {
					return err
				}
				if err = fetchMissing(pt, mpt, mt, denyIds, srcUrl, mainDstUrl, dstUrl, verbose); err != nil {
					return err
				}
			}
			return nil
		} else {
			return fetchItems(ids, pt, mt, srcUrl, dstUrl, verbose)
		}
	}
}

func fetchItem(
	id string,
	pt vangogh_types.ProductType,
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
	pt vangogh_types.ProductType,
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

func fetchMissing(
	detailPt, mainPt vangogh_types.ProductType,
	mt gog_media.Media,
	denyIds []string,
	sourceUrl vangogh_urls.ProductTypeUrl,
	mainDestUrl, detailDestUrl string,
	verbose bool) error {
	log.Printf("get missing %s (%s) for %s", detailPt, mt, mainPt)

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
	pt vangogh_types.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) error {

	// TODO: move to vangogh_types
	switch pt {
	case vangogh_types.Details:
		break
	case vangogh_types.ApiProductsV1:
		break
	case vangogh_types.ApiProductsV2:
		break
	default:
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
