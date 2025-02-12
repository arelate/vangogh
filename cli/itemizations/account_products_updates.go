package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"strconv"
)

func AccountProductsUpdates() ([]string, error) {

	apua := nod.Begin(" finding %s updates...", vangogh_integration.AccountProducts)
	defer apua.Done()

	updatesSet := make(map[string]bool)
	vrAccountPages, err := vangogh_integration.NewProductReader(vangogh_integration.AccountPage)
	if err != nil {
		return nil, err
	}

	for page := range vrAccountPages.Keys() {
		accountPage, err := vrAccountPages.AccountPage(page)
		if err != nil {
			return nil, err
		}
		for _, ap := range accountPage.Products {
			if ap.Updates > 0 ||
				ap.IsNew {
				nod.Log("%s #%d Updates, isNew: %d, %v", vangogh_integration.AccountProducts, ap.Id, ap.Updates, ap.IsNew)
				updatesSet[strconv.Itoa(ap.Id)] = true
			}
		}
	}

	apua.EndWithResult(itemizationResult(updatesSet))

	return maps.Keys(updatesSet), nil
}
