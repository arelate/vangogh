package fetch

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
	"net/http"
)

func Items(
	ids []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media,
	verbose bool) error {

	if !vangogh_products.SupportsGetItems(pt) {
		return fmt.Errorf("getting %s is not supported", pt)
	}

	if verbose {
		log.Printf("getting data for ids: %v", ids)
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}

	sourceUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return err
	}

	httpClient, err := internal.HttpClient()
	if err != nil {
		return err
	}

	vs, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return err
	}

	for i, id := range ids {
		fmt.Printf("\rgetting %s (%s) %d/%d...", pt, mt, i+1, len(ids))
		_, err := getItem(id, mt, httpClient, vs, sourceUrl, destUrl, verbose)
		if err != nil {
			log.Printf("error getting %s (%s) %s: %v", pt, mt, id, err)
		}
	}

	if len(ids) > 0 {
		fmt.Println("done")
	}

	return nil
}

func getItem(
	id string,
	mt gog_media.Media,
	httpClient *http.Client,
	vs *kvas.ValueSet,
	sourceUrl vangogh_urls.ProductTypeUrl,
	destUrl string,
	verbose bool) (io.Reader, error) {

	u := sourceUrl(id, mt)
	if verbose {
		log.Printf("source url %s", u)
	}

	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("vangogh: unexpected status: %s", resp.Status)
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
