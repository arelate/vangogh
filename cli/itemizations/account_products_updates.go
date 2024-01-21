package itemizations

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"strconv"
)

func AccountProductsUpdates() (map[string]bool, error) {

	apua := nod.Begin(" finding %s updates...", vangogh_local_data.AccountProducts)
	defer apua.End()

	updatesSet := make(map[string]bool)
	vrAccountPages, err := vangogh_local_data.NewProductReader(vangogh_local_data.AccountPage)
	if err != nil {
		return updatesSet, apua.EndWithError(err)
	}

	for _, page := range vrAccountPages.Keys() {
		accountPage, err := vrAccountPages.AccountPage(page)
		if err != nil {
			return updatesSet, apua.EndWithError(err)
		}
		for _, ap := range accountPage.Products {
			if ap.Updates > 0 ||
				ap.IsNew {
				nod.Log("%s #%d Updates, isNew: %d, %v", vangogh_local_data.AccountProducts, ap.Id, ap.Updates, ap.IsNew)
				updatesSet[strconv.Itoa(ap.Id)] = true
			}
		}
	}

	apua.EndWithResult(itemizationResult(updatesSet))

	return updatesSet, nil
}
