package cmd

import (
	"bytes"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/vangogh/internal"
	"io"
	"log"
)

func getItems(
	ids []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	verbose bool) error {

	if !vangogh_products.SupportsGetItems(pt) {
		return fmt.Errorf("getting %s data is not supported", pt)
	}

	if verbose {
		log.Printf("get data for ids: %v", ids)
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}

	sourceUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return err
	}

	for _, id := range ids {
		_, err := getItem(id, pt, mt, sourceUrl, destUrl, verbose)
		if err != nil {
			log.Printf("couldn't get data for %s (%s) %s: %v", pt, mt, id, err)
		}
	}
	return nil
}

func getItem(
	id string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) (io.Reader, error) {

	fmt.Printf("get %s (%s) data %s\n", pt, mt, id)

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
