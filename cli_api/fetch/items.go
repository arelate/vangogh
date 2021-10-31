package fetch

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/http_client"
	"net/url"
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

	sourceUrl, err := vangogh_urls.RemoteProductsUrl(pt)
	if err != nil {
		return ia.EndWithError(err)
	}

	httpClient, err := http_client.Default()
	if err != nil {
		return ia.EndWithError(err)
	}

	urls := make([]*url.URL, len(ids))
	idStr := make([]string, len(ids))

	for i := 0; i < len(ids); i++ {
		urls[i], idStr[i] = sourceUrl(ids[i], mt), ids[i]
	}

	kis, err := NewKvasIndexSetter(pt, mt, idStr)
	if err != nil {
		return ia.EndWithError(err)
	}

	if err := dolo.GetSet(urls, kis, httpClient, ia); err != nil {
		return ia.EndWithError(err)
	}

	ia.EndWithResult("done")

	return nil
}
