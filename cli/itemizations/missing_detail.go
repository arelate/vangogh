package itemizations

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
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
	detailPt, mainPt vangogh_local_data.ProductType,
	since int64) ([]string, error) {

	//api-products-v2 provides
	//includes-games, is-included-by-games,
	//requires-games, is-required-by-games
	if mainPt == vangogh_local_data.ApiProductsV2 &&
		detailPt == vangogh_local_data.ApiProductsV2 {
		lgs, err := linkedGames(since)
		if err != nil {
			return nil, err
		}
		return lgs, nil
	}

	//licences give a signal when DLC has been purchased, this would add
	//required (base) game details to the updates
	if mainPt == vangogh_local_data.LicenceProducts &&
		detailPt == vangogh_local_data.Details {
		rgs, err := RequiredAndIncluded(since)
		if err != nil {
			return nil, err
		}
		return rgs, nil
	}

	mda := nod.Begin(" finding missing %s for %s...", detailPt, mainPt)
	defer mda.End()

	missingIdSet := make(map[string]bool)

	mainDestUrl, err := vangogh_local_data.AbsLocalProductTypeDir(mainPt)
	if err != nil {
		return nil, mda.EndWithError(err)
	}

	detailDestUrl, err := vangogh_local_data.AbsLocalProductTypeDir(detailPt)
	if err != nil {
		return nil, mda.EndWithError(err)
	}

	kvMain, err := kevlar.NewKeyValues(mainDestUrl, kevlar.JsonExt)
	if err != nil {
		return nil, mda.EndWithError(err)
	}

	kvDetail, err := kevlar.NewKeyValues(detailDestUrl, kevlar.JsonExt)
	if err != nil {
		return nil, mda.EndWithError(err)
	}

	keys, err := kvMain.Keys()
	if err != nil {
		return nil, mda.EndWithError(err)
	}
	for _, id := range keys {
		has, err := kvDetail.Has(id)
		if err != nil {
			return nil, mda.EndWithError(err)
		}
		if !has {
			nod.Log("adding missing %s: #%s", detailPt, id)
			missingIdSet[id] = true
		}
	}

	mda.EndWithResult(itemizationResult(missingIdSet))

	if mainPt == vangogh_local_data.AccountProducts &&
		detailPt == vangogh_local_data.Details {
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
