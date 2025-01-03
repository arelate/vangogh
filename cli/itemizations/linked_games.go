package itemizations

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func linkedGames(modifiedAfter int64) ([]string, error) {

	lga := nod.Begin(" finding missing linked %s...", vangogh_local_data.ApiProductsV2)
	defer lga.End()

	missingSet := make(map[string]bool)

	vrApv2, err := vangogh_local_data.NewProductReader(vangogh_local_data.ApiProductsV2)
	if err != nil {
		return nil, lga.EndWithError(err)
	}

	modifiedApv2, err := vrApv2.CreatedOrUpdatedAfter(modifiedAfter)
	if err != nil {
		return nil, lga.EndWithError(err)
	}
	if len(modifiedApv2) > 0 {
		nod.Log("modified %s: %v", vangogh_local_data.ApiProductsV2, modifiedApv2)
	}

	for _, id := range modifiedApv2 {

		// have to use product reader and not reductions here, since redux wouldn't be ready
		// while we're still getting data. Attempting to minimize the impact by only querying
		// new or updated api-product-v2 items since start to the sync
		apv2, err := vrApv2.ApiProductV2(id)

		if err != nil {
			return nil, lga.EndWithError(err)
		}

		gig := apv2.GetIncludesGames()
		if len(gig) > 0 {
			nod.Log("%s #%s includes-games: %v", vangogh_local_data.ApiProductsV2, id, gig)
		}
		lgs := gig

		giiig := apv2.GetIsIncludedInGames()
		if len(giiig) > 0 {
			nod.Log("%s #%s is-included-in-games: %v", vangogh_local_data.ApiProductsV2, id, giiig)
		}
		lgs = append(lgs, giiig...)

		grg := apv2.GetRequiresGames()
		if len(grg) > 0 {
			nod.Log("%s #%s requires-games: %v", vangogh_local_data.ApiProductsV2, id, grg)
		}
		lgs = append(lgs, grg...)

		girbg := apv2.GetIsRequiredByGames()
		if len(girbg) > 0 {
			nod.Log("%s #%s is-required-by-games: %v", vangogh_local_data.ApiProductsV2, id, girbg)
		}
		lgs = append(lgs, girbg...)

		for _, lid := range lgs {
			if has, err := vrApv2.Has(lid); err == nil && !has {
				missingSet[lid] = true
			} else if err != nil {
				return nil, lga.EndWithError(err)
			}
		}
	}

	lga.EndWithResult(itemizationResult(missingSet))

	return maps.Keys(missingSet), nil
}
