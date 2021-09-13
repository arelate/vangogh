package fetch

import (
	"bytes"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/vangogh/cmd/http_client"
	"io"
	"log"
	"net/http"
)

func Items(
	ids []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media) error {

	if !vangogh_products.SupportsGetItems(pt) {
		return fmt.Errorf("getting %s is not supported", pt)
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}

	sourceUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return err
	}

	httpClient, err := http_client.Default()
	if err != nil {
		return err
	}

	vs, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return err
	}

	for i, id := range ids {
		fmt.Printf("\rfetching %s (%s) %d/%d... ", pt, mt, i+1, len(ids))
		_, err := getItem(id, mt, httpClient, vs, sourceUrl, destUrl)
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
	destUrl string) (io.Reader, error) {

	u := sourceUrl(id, mt)

	resp, err := httpClient.Get(u.String())
	if err != nil {
		return nil, err
	}

	if resp.StatusCode < 200 || resp.StatusCode > 299 {
		return nil, fmt.Errorf("vangogh: unexpected status: %s", resp.Status)
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
