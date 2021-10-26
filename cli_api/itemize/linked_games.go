package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
)

func linkedGames(modifiedAfter int64) (gost.StrSet, error) {

	lga := nod.Begin(" finding missing linked %s...", vangogh_products.ApiProductsV2)
	defer lga.End()

	missingSet := gost.NewStrSet()

	//currently, api-products-v2 support only gog_media.Game, and since this method is exclusively
	//using api-products-v2 we're fine specifying media directly and not taking as a parameter
	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	if err != nil {
		return missingSet, lga.EndWithError(err)
	}

	for _, id := range vrApv2.ModifiedAfter(modifiedAfter, false) {

		// have to use product reader and not extracts here, since extracts wouldn't be ready
		// while we're still getting data. Attempting to minimize the impact by only querying
		// new or updated api-product-v2 items since start to the sync
		apv2, err := vrApv2.ApiProductV2(id)

		if err != nil {
			return missingSet, lga.EndWithError(err)
		}

		lgs := apv2.GetIncludesGames()
		lgs = append(lgs, apv2.GetIsIncludedInGames()...)
		lgs = append(lgs, apv2.GetRequiresGames()...)
		lgs = append(lgs, apv2.GetIsRequiredByGames()...)

		for _, lid := range lgs {
			if !vrApv2.Contains(lid) {
				missingSet.Add(lid)
			}
		}
	}

	lga.EndWithResult(itemizationResult(missingSet))

	return missingSet, nil
}
