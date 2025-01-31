package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func gamesDbCatalogGames(since int64, missing bool) ([]string, error) {

	mcga := nod.Begin(" finding missing %s products of GAME type...", vangogh_integration.CatalogProducts)
	defer mcga.End()

	missingSet := make(map[string]bool)

	vrGamesDbProducts, err := vangogh_integration.NewProductReader(vangogh_integration.GamesDbGogProducts)
	if err != nil {
		return nil, mcga.EndWithError(err)
	}

	vrCatalogProducts, err := vangogh_integration.NewProductReader(vangogh_integration.CatalogProducts)
	if err != nil {
		return nil, mcga.EndWithError(err)
	}

	modifiedCatalogProducts := vrCatalogProducts.CreatedOrUpdatedAfter(since)

	for id := range modifiedCatalogProducts {

		// have to use product reader and not reductions here, since redux wouldn't be ready
		// while we're still getting data. Attempting to minimize the impact by only querying
		// new or updated api-product-v2 items since start to the sync
		cp, err := vrCatalogProducts.CatalogProduct(id)
		if err != nil {
			return nil, mcga.EndWithError(err)
		}

		if cpt := cp.GetProductType(); cpt != "GAME" {
			continue
		}

		if missing {
			if vrGamesDbProducts.Has(id) {
				continue
			}
		}

		missingSet[id] = true

	}

	mcga.EndWithResult(itemizationResult(missingSet))

	return maps.Keys(missingSet), nil
}
