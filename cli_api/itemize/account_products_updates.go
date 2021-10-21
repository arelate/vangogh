package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"strconv"
)

func AccountProductsUpdates(mt gog_media.Media) (gost.StrSet, error) {
	updatesSet := gost.NewStrSet()
	vrAccountPages, err := vangogh_values.NewReader(vangogh_products.AccountPage, mt)
	if err != nil {
		return updatesSet, err
	}

	for _, page := range vrAccountPages.All() {
		accountPage, err := vrAccountPages.AccountPage(page)
		if err != nil {
			return updatesSet, err
		}
		for _, ap := range accountPage.Products {
			if ap.Updates > 0 ||
				ap.IsNew {
				updatesSet.Add(strconv.Itoa(ap.Id))
			}
		}
	}

	return updatesSet, nil
}

//func AccountProductsUpdatesOld(mt gog_media.Media, since int64) (gost.StrSet, error) {
//	updatesSet := gost.NewStrSet()
//	vrAccountProducts, err := vangogh_values.NewReader(vangogh_products.AccountProducts, mt)
//	if err != nil {
//		return updatesSet, err
//	}
//
//	var accountProducts []string
//	if since > 0 {
//		accountProducts = vrAccountProducts.ModifiedAfter(since, false)
//	} else {
//		accountProducts = vrAccountProducts.All()
//	}
//
//	for _, id := range accountProducts {
//		ap, err := vrAccountProducts.AccountProduct(id)
//		if err != nil {
//			return updatesSet, err
//		}
//		if ap.Updates > 0 ||
//			ap.IsNew {
//			updatesSet.Add(id)
//		}
//	}
//
//	return updatesSet, nil
//}
