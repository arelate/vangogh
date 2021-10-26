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

	raia := nod.Begin(" finding DLCs missing required base product...")
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

	for _, id := range vrLicences.CreatedAfter(createdAfter) {
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

		for _, reqGame := range apv2.GetRequiresGames() {
			newLicSet.Add(reqGame)
		}

		for _, inclGame := range apv2.GetIncludesGames() {
			newLicSet.Add(inclGame)
		}
	}

	raia.EndWithResult(itemizationResult(newLicSet))

	return newLicSet, nil
}
