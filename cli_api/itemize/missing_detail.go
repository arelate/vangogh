package itemize

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
)

func itemizationResult(set gost.StrSet) string {
	if set.Len() == 0 {
		return "found nothing"
	} else {
		return fmt.Sprintf("found %d", set.Len())
	}
}

func missingDetail(
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	since int64) (gost.StrSet, error) {

	//api-products-v2 provides
	//includes-games, is-included-by-games,
	//requires-games, is-required-by-games
	if mainPt == vangogh_products.ApiProductsV2 &&
		detailPt == vangogh_products.ApiProductsV2 {
		lg, err := linkedGames(since)
		if err != nil {
			return lg, err
		}
		return lg, nil
	}

	//licences give a signal when DLC has been purchased, this would add
	//required (base) game details to the updates
	if mainPt == vangogh_products.LicenceProducts &&
		detailPt == vangogh_products.Details {
		rg, err := RequiredAndIncluded(since)
		if err != nil {
			return rg, err
		}
		return rg, nil
	}

	mda := nod.Begin(" finding missing %s for %s...", detailPt, mainPt)
	defer mda.End()

	missingIdSet := gost.NewStrSet()

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return missingIdSet, mda.EndWithError(err)
	}

	detailDestUrl, err := vangogh_urls.LocalProductsDir(detailPt, mt)
	if err != nil {
		return missingIdSet, mda.EndWithError(err)
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return missingIdSet, mda.EndWithError(err)
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
	if err != nil {
		return missingIdSet, mda.EndWithError(err)
	}

	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) {
			nod.Log("adding missing %s: #%s", detailPt, id)
			missingIdSet.Add(id)
		}
	}

	mda.EndWithResult(itemizationResult(missingIdSet))

	if mainPt == vangogh_products.AccountProducts &&
		detailPt == vangogh_products.Details {
		updatedAccountProducts, err := AccountProductsUpdates(mt)
		if err != nil {
			return missingIdSet, err
		}
		for uapId := range updatedAccountProducts {
			missingIdSet.Add(uapId)
		}
	}

	return missingIdSet, nil
}
