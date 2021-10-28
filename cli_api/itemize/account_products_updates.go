package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"strconv"
)

func AccountProductsUpdates(mt gog_media.Media) (gost.StrSet, error) {

	apua := nod.Begin(" finding %s updates...", vangogh_products.AccountProducts)
	defer apua.End()

	updatesSet := gost.NewStrSet()
	vrAccountPages, err := vangogh_values.NewReader(vangogh_products.AccountPage, mt)
	if err != nil {
		return updatesSet, apua.EndWithError(err)
	}

	for _, page := range vrAccountPages.All() {
		accountPage, err := vrAccountPages.AccountPage(page)
		if err != nil {
			return updatesSet, apua.EndWithError(err)
		}
		for _, ap := range accountPage.Products {
			if ap.Updates > 0 ||
				ap.IsNew {
				nod.Log("%s #%d Updates, isNew: %d, %v", vangogh_products.AccountProducts, ap.Id, ap.Updates, ap.IsNew)
				updatesSet.Add(strconv.Itoa(ap.Id))
			}
		}
	}

	apua.EndWithResult(itemizationResult(updatesSet))

	return updatesSet, nil
}
