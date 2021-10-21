package itemize

import (
	"fmt"
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
			fmt.Printf(" finding Modified %s... ", pt)
			// Modified main product type items
			modifiedIds, err := Modified(modifiedAfter, mainPt, mt)
			if err != nil {
				return idSet, err
			}
			printFoundAndAll(modifiedIds)
			idSet.AddSet(modifiedIds)
		}
	}

	return idSet, nil
}
