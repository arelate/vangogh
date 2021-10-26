package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/boggydigital/gost"
)

func All(
	idSet gost.StrSet,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) (gost.StrSet, error) {

	for _, mainPt := range vangogh_products.MainTypes(pt) {
		if missing {
			missingIds, err := missingDetail(pt, mainPt, mt, modifiedAfter)
			if err != nil {
				return idSet, err
			}
			idSet.AddSet(missingIds)
		}
		if updated {
			modifiedIds, err := Modified(modifiedAfter, mainPt, mt)
			if err != nil {
				return idSet, err
			}
			idSet.AddSet(modifiedIds)
		}
	}

	return idSet, nil
}
