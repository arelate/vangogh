package itemizations

import (
	"github.com/arelate/vangogh_local_data"
)

func All(
	idSet map[string]bool,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_local_data.ProductType) (map[string]bool, error) {

	for _, mainPt := range vangogh_local_data.MainProductTypes(pt) {
		if missing {
			missingIds, err := missingDetail(pt, mainPt, modifiedAfter)
			if err != nil {
				return idSet, err
			}
			for id := range missingIds {
				idSet[id] = true
			}
		}
		if updated {
			modifiedIds, err := Modified(modifiedAfter, mainPt)
			if err != nil {
				return idSet, err
			}
			for id := range modifiedIds {
				idSet[id] = true
			}
		}
	}

	return idSet, nil
}
