package fetch

import (
	"bytes"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"io"
)

func Items(
	ids []string,
	pt vangogh_products.ProductType,
	mt gog_media.Media) error {

	ia := nod.NewProgress(" fetching %s (%s)...", pt, mt)
	defer ia.End()

	ia.TotalInt(len(ids))

	if !vangogh_products.SupportsGetItems(pt) {
		return ia.EndWithError(fmt.Errorf("getting %s is not supported", pt))
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return ia.EndWithError(err)
	}

	sourceUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return ia.EndWithError(err)
	}

	httpClient, err := http_client.Default()
	if err != nil {
		return ia.EndWithError(err)
	}

	vs, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return ia.EndWithError(err)
	}

	for _, id := range ids {

		u := sourceUrl(id, mt)

		resp, err := httpClient.Get(u.String())
		if err != nil {
			return ia.EndWithError(err)
		}

		if resp.StatusCode < 200 || resp.StatusCode > 299 {
			return ia.EndWithError(fmt.Errorf("unexpected status: %s", resp.Status))
		}

		var b bytes.Buffer
		tr := io.TeeReader(resp.Body, &b)

		if err = vs.Set(id, tr); err != nil {
			return ia.EndWithError(err)
		}

		if err := resp.Body.Close(); err != nil {
			return ia.EndWithError(err)
		}

		if err != nil {
			ia.Error(fmt.Errorf("error getting %s (%s) %s: %v", pt, mt, id, err))
		}

		ia.Increment()
	}

	//if len(ids) > 0 {
	ia.EndWithResult("done")
	//}

	return nil
}
