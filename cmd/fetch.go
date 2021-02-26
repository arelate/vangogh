package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_auth"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/vangogh/internal"
	"io"
	"log"
	"strconv"
)

func fetchItem(
	id string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string) (io.Reader, error) {

	log.Printf("fetching %s (%s) %s\n", pt, mt, id)

	httpClient, err := internal.HttpClient()
	if err != nil {
		return nil, err
	}

	u := sourceUrl(id, mt)
	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("error fetching read closer at %s: %s", u.String(), resp.Status)
	}

	vs, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return resp.Body, err
	}

	var b bytes.Buffer
	tr := io.TeeReader(resp.Body, &b)

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
	mt gog_types.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string) error {
	totalPages := 1
	for pp := 1; pp <= totalPages; pp++ {

		rdr, err := fetchItem(strconv.Itoa(pp), pt, mt, sourceUrl, destUrl)
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
	pt, mainPt vangogh_types.ProductType,
	mt gog_types.Media,
	denyIds []string,
	sourceUrl vangogh_urls.ProductTypeUrl,
	mainDestUrl, detailDestUrl string) error {
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
		if err := fetchItems(missingIds, pt, mt, sourceUrl, detailDestUrl); err != nil {
			//log.Println(err)
			return err
		}
	} else {
		log.Printf("no missing %s for %s (%s)\n", pt, mainPt, mt)
	}

	return nil
}

func fetchItems(
	ids []string,
	pt vangogh_types.ProductType,
	mt gog_types.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string) error {

	switch pt {
	case vangogh_types.Details:
		break
	case vangogh_types.ApiProducts:
		break
	default:
		return fmt.Errorf("fetching items of type %s is not supported", pt)
	}

	for _, id := range ids {
		_, err := fetchItem(id, pt, mt, sourceUrl, destUrl)
		if err != nil {
			log.Println(err)
			//return err
		}
	}
	return nil
}

func Fetch(ids []string, denyIds []string, pt vangogh_types.ProductType, mt gog_types.Media, missing bool) error {

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	if vangogh_types.RequiresAuth(pt) {
		li, err := gog_auth.LoggedIn(httpClient)
		if err != nil {
			return err
		}

		if !li {
			log.Fatalf("fetching type %s requires authenticated session", pt)
		}
	}

	dstUrl, err := vangogh_urls.DstProductTypeUrl(pt, mt)
	if err != nil {
		return err
	}

	srcUrl, err := vangogh_urls.SrcProductTypeUrl(pt)
	if err != nil {
		return err
	}

	if vangogh_types.HasPages(pt) {
		if err := fetchPages(pt, mt, srcUrl, dstUrl); err != nil {
			return err
		}

		return split(pt, mt)
	} else {
		if missing {
			for _, mpt := range vangogh_types.MainProductTypes(pt) {
				mainDstUrl, err := vangogh_urls.DstProductTypeUrl(mpt, mt)
				if err != nil {
					return err
				}
				if err = fetchMissing(pt, mpt, mt, denyIds, srcUrl, mainDstUrl, dstUrl); err != nil {
					return err
				}
			}
			return nil
		} else {
			return fetchItems(ids, pt, mt, srcUrl, dstUrl)
		}
	}
}
