package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
)

func All(
	ids []string,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_integration.ProductType) ([]string, error) {

	for _, mainPt := range vangogh_integration.MainProductTypes(pt) {
		if missing {
			missingIds, err := missingDetail(pt, mainPt, modifiedAfter)
			if err != nil {
				return nil, err
			}
			ids = append(ids, missingIds...)
		}
		if updated {
			// don't update gamesdb-products on catalog-products updates
			if pt == vangogh_integration.GamesDBProducts {
				modifiedCatalogGameProductsIds, err := gamesDbCatalogGames(modifiedAfter, false)
				if err != nil {
					return nil, err
				}
				ids = append(ids, modifiedCatalogGameProductsIds...)
			} else {
				modifiedIds, err := Modified(modifiedAfter, mainPt)
				if err != nil {
					return nil, err
				}
				ids = append(ids, modifiedIds...)
			}
		}
	}

	return ids, nil
}
