package itemizations

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
)

func itemizationResult(idSet map[string]bool) string {
	if len(idSet) == 0 {
		return "found nothing"
	} else {
		return fmt.Sprintf("found %d", len(idSet))
	}
}

func missingDetail(
	detailPt, mainPt vangogh_integration.ProductType,
	since int64) ([]string, error) {

	// gamesdb products don't exist for DLC, PACK product types,
	// only exist for GAME - so we need to filter them specially
	if mainPt == vangogh_integration.CatalogProducts &&
		detailPt == vangogh_integration.GamesDbGogProducts {
		return gamesDbCatalogGames(since, true)
	}

	//api-products-v2 provides
	//includes-games, is-included-by-games,
	//requires-games, is-required-by-games
	if mainPt == vangogh_integration.ApiProductsV2 &&
		detailPt == vangogh_integration.ApiProductsV2 {
		return linkedGames(since)
	}

	//licences give a signal when DLC has been purchased, this would add
	//required (base) game details to the updates
	//if mainPt == vangogh_integration.LicenceProducts &&
	//	detailPt == vangogh_integration.Details {
	//	return RequiredAndIncluded(since)
	//}

	mda := nod.Begin(" finding missing %s for %s...", detailPt, mainPt)
	defer mda.EndWithResult("done")

	missingIdSet := make(map[string]bool)

	mainDestUrl, err := vangogh_integration.AbsProductTypeDir(mainPt)
	if err != nil {
		return nil, err
	}

	detailDestUrl, err := vangogh_integration.AbsProductTypeDir(detailPt)
	if err != nil {
		return nil, err
	}

	kvMain, err := kevlar.New(mainDestUrl, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	kvDetail, err := kevlar.New(detailDestUrl, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	for id := range kvMain.Keys() {
		if !kvDetail.Has(id) {
			nod.Log("adding missing %s: #%s", detailPt, id)
			missingIdSet[id] = true
		}
	}

	mda.EndWithResult(itemizationResult(missingIdSet))

	if mainPt == vangogh_integration.AccountProducts &&
		detailPt == vangogh_integration.Details {
		updatedAccountProducts, err := AccountProductsUpdates()
		if err != nil {
			return nil, err
		}
		for _, uapId := range updatedAccountProducts {
			missingIdSet[uapId] = true
		}
	}

	return maps.Keys(missingIdSet), nil
}
