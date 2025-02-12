package fetchers

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/dolo"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
)

// Items fetches all individual data items (details, api-products-v1/v2) using provided ids
func Items(
	ids []string,
	pt vangogh_integration.ProductType,
	httpClient *http.Client) error {

	ia := nod.NewProgress(" fetching %s...", pt)
	defer ia.Done()

	if !vangogh_integration.IsGetItemsSupported(pt) {
		return fmt.Errorf("getting %s is not supported", pt)
	}

	ia.TotalInt(len(ids))

	rdx, err := vangogh_integration.NewReduxReader(
		vangogh_integration.SteamAppIdProperty,
		vangogh_integration.PCGWPageIdProperty,
		vangogh_integration.HLTBBuildIdProperty,
		vangogh_integration.HLTBIdProperty)
	if err != nil {
		return err
	}

	//since we know how many ids need to be fetched, allocate URLs and idStrs to that number
	urls, idStr := make([]*url.URL, len(ids)), make([]string, len(ids))

	up, err := vangogh_integration.NewUrlProvider(pt, rdx)
	if err != nil {
		return err
	}

	for i := 0; i < len(ids); i++ {
		urls[i], idStr[i] = up.Url(ids[i]), ids[i]
	}

	kis, err := NewIndexSetter(pt, idStr)
	if err != nil {
		return err
	}

	dc := dolo.NewClient(httpClient, dolo.Defaults())

	if errs := dc.GetSet(urls, kis, ia, false); len(errs) > 0 {
		for ui, e := range errs {
			ia.Error(fmt.Errorf("GetSet %s error: %s", urls[ui], e.Error()))
		}
	}

	return nil
}
