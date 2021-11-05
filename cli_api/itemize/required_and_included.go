package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
)

//RequiredAndIncluded enumerates all base products for a newly acquired DLCs
func RequiredAndIncluded(createdAfter int64) (gost.StrSet, error) {

	raia := nod.Begin(" finding new DLCs missing required base product...")
	defer raia.End()

	newLicSet := gost.NewStrSet()

	vrLicences, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, gog_media.Game)
	if err != nil {
		return nil, raia.EndWithError(err)
	}

	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	if err != nil {
		return nil, raia.EndWithError(err)
	}

	newLicences := vrLicences.CreatedAfter(createdAfter)
	if len(newLicences) > 0 {
		nod.Log("new %s: %v", vangogh_products.LicenceProducts, newLicences)
	}

	for _, id := range newLicences {
		// it's not guaranteed that a license would have an existing api-products-v2
		if !vrApv2.Contains(id) {
			continue
		}
		//like in itemizeMissingIncludesGames, we can't use extracts here,
		//because we're in process of getting data and would rather query api-products-v2 directly.
		//the performance impact is expected to be minimal since we're only loading api-products-v2
		//for newly acquired licences.
		apv2, err := vrApv2.ApiProductV2(id)
		if err != nil {
			return nil, raia.EndWithError(err)
		}

		grg := apv2.GetRequiresGames()
		if len(grg) > 0 {
			nod.Log("%s #%s requires-games: %v", vangogh_products.ApiProductsV2, id, grg)
		}
		for _, reqGame := range grg {
			newLicSet.Add(reqGame)
		}

		gig := apv2.GetIncludesGames()
		if len(gig) > 0 {
			nod.Log("%s #%s includes-games: %v", vangogh_products.ApiProductsV2, id, gig)
		}

		for _, inclGame := range gig {
			newLicSet.Add(inclGame)
		}
	}

	raia.EndWithResult(itemizationResult(newLicSet))

	return newLicSet, nil
}
