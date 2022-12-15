package fetchers

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
)

// Items fetches all individual data items (details, api-products-v1/v2) using provided ids
func Items(
	ids []string,
	pt vangogh_local_data.ProductType,
	httpClient *http.Client) error {

	ia := nod.NewProgress(" fetching %s...", pt)
	defer ia.End()

	if !vangogh_local_data.IsGetItemsSupported(pt) {
		return ia.EndWithError(fmt.Errorf("getting %s is not supported", pt))
	}

	ia.TotalInt(len(ids))

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.SteamAppIdProperty,
		vangogh_local_data.PCGWPageIdProperty,
		vangogh_local_data.HLTBBuildIdProperty,
		vangogh_local_data.HLTBIdProperty)
	if err != nil {
		return err
	}

	//since we know how many ids need to be fetched, allocate URLs and idStrs to that number
	urls, idStr := make([]*url.URL, len(ids)), make([]string, len(ids))

	up, err := vangogh_local_data.NewUrlProvider(pt, rxa)
	if err != nil {
		return ia.EndWithError(err)
	}

	for i := 0; i < len(ids); i++ {
		urls[i], idStr[i] = up.Url(ids[i]), ids[i]
	}

	kis, err := NewKvasIndexSetter(pt, idStr)
	if err != nil {
		return ia.EndWithError(err)
	}

	dc := dolo.NewClient(httpClient, dolo.Defaults())

	if err := dc.GetSet(urls, kis, ia); err != nil {
		return ia.EndWithError(err)
	}

	ia.EndWithResult("done")

	return nil
}
