package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

// RequiredAndIncluded enumerates all base products for a newly acquired DLCs
func RequiredAndIncluded(createdAfter int64) ([]string, error) {

	raia := nod.Begin(" finding new DLCs missing required base product...")
	defer raia.End()

	newLicSet := make(map[string]bool)

	vrLicences, err := vangogh_integration.NewProductReader(vangogh_integration.LicenceProducts)
	if err != nil {
		return nil, raia.EndWithError(err)
	}

	vrApv2, err := vangogh_integration.NewProductReader(vangogh_integration.ApiProductsV2)
	if err != nil {
		return nil, raia.EndWithError(err)
	}

	newLicences, err := vrLicences.CreatedAfter(createdAfter)
	if err != nil {
		return nil, raia.EndWithError(err)
	}
	if len(newLicences) > 0 {
		nod.Log("new %s: %v", vangogh_integration.LicenceProducts, newLicences)
	}

	for _, id := range newLicences {
		// it's not guaranteed that a license would have an existing api-products-v2
		has, err := vrApv2.Has(id)
		if err != nil {
			return nil, raia.EndWithError(err)
		}
		if !has {
			continue
		}
		//like in itemizeMissingIncludesGames, we can't use redux here,
		//because we're in process of getting data and would rather query api-products-v2 directly.
		//the performance impact is expected to be minimal since we're only loading api-products-v2
		//for newly acquired licences.
		apv2, err := vrApv2.ApiProductV2(id)
		if err != nil {
			return nil, raia.EndWithError(err)
		}

		grg := apv2.GetRequiresGames()
		if len(grg) > 0 {
			nod.Log("%s #%s requires-games: %v", vangogh_integration.ApiProductsV2, id, grg)
		}
		for _, reqGame := range grg {
			newLicSet[reqGame] = true
		}

		gig := apv2.GetIncludesGames()
		if len(gig) > 0 {
			nod.Log("%s #%s includes-games: %v", vangogh_integration.ApiProductsV2, id, gig)
		}

		for _, inclGame := range gig {
			newLicSet[inclGame] = true
		}
	}

	//newLicSet contains all product types at the moment, we need to filter to GAME types only,
	//since other types won't have account-products / details data available remotely
	for id := range newLicSet {
		has, err := vrApv2.Has(id)
		if err != nil {
			return nil, raia.EndWithError(err)
		}
		if !has {
			delete(newLicSet, id)
			continue
		}
		apv2, err := vrApv2.ApiProductV2(id)
		if err != nil {
			return nil, raia.EndWithError(err)
		}
		if apv2.Embedded.ProductType != "GAME" {
			delete(newLicSet, id)
		}
	}

	raia.EndWithResult(itemizationResult(newLicSet))

	return maps.Keys(newLicSet), nil
}
