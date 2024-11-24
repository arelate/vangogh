package itemizations

import (
	"github.com/arelate/vangogh_local_data"
)

func All(
	ids []string,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_local_data.ProductType) ([]string, error) {

	for _, mainPt := range vangogh_local_data.MainProductTypes(pt) {
		if missing {
			missingIds, err := missingDetail(pt, mainPt, modifiedAfter)
			if err != nil {
				return nil, err
			}
			ids = append(ids, missingIds...)
		}
		if updated {
			modifiedIds, err := Modified(modifiedAfter, mainPt)
			if err != nil {
				return nil, err
			}
			ids = append(ids, modifiedIds...)
		}
	}

	return ids, nil
}
