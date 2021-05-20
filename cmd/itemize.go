package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/kvas"
)

func itemizeAll(
	ids map[string]bool,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) (map[string]bool, error) {

	if ids == nil {
		ids = make(map[string]bool, 0)
	}

	for _, mainPt := range vangogh_products.MainTypes(pt) {
		if missing {
			missingIds, err := itemizeMissing(pt, mainPt, mt, modifiedAfter)
			if err != nil {
				return ids, err
			}
			if len(missingIds) == 0 {
				fmt.Printf("no missing %s data for %s (%s)\n", pt, mainPt, mt)
			}
			for missId := range missingIds {
				ids[missId] = true
			}
		}
		if updated {
			updatedIds, err := itemizeUpdated(modifiedAfter, mainPt, mt)
			if err != nil {
				return ids, err
			}
			if len(updatedIds) == 0 {
				fmt.Printf("no updated %s data for %s (%s)\n", pt, mainPt, mt)
			}
			for updId := range updatedIds {
				ids[updId] = true
			}
		}
	}

	return ids, nil
}

func itemizeMissing(
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	modifiedAfter int64) (map[string]bool, error) {

	//api-products-v2 provides
	//includes-games, is-included-by-games,
	//requires-games, is-required-by-games
	if mainPt == vangogh_products.ApiProductsV2 &&
		detailPt == vangogh_products.ApiProductsV2 {
		fmt.Printf("checking missing linked %s\n", mainPt)
		return itemizeAPV2LinkedGames(modifiedAfter)
	}

	//licences give a signal when DLC has been purchased, this would add
	//required (base) game details to the updates
	if mainPt == vangogh_products.LicenceProducts &&
		detailPt == vangogh_products.Details {
		fmt.Printf("checking missing required %s for modified %s\n", detailPt, mainPt)
		return itemizeRequiredGames(modifiedAfter, mt)
	}

	//TODO: convert this into map[string]bool to avoid duplicates
	missingIds := make(map[string]bool, 0)

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return missingIds, err
	}

	detailDestUrl, err := vangogh_urls.LocalProductsDir(detailPt, mt)
	if err != nil {
		return missingIds, err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return missingIds, err
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
	if err != nil {
		return missingIds, err
	}
	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) {
			missingIds[id] = true
		}
	}

	if mainPt == vangogh_products.AccountProducts &&
		detailPt == vangogh_products.Details {
		fmt.Printf("checking %s updates to update %s\n", mainPt, detailPt)
		updatedAccountProducts, err := itemizeAccountProductsUpdates(mt)
		if err != nil {
			return missingIds, err
		}
		for uapId, ok := range updatedAccountProducts {
			if !ok {
				continue
			}
			missingIds[uapId] = true
		}
	}

	return missingIds, nil
}

func itemizeAccountProductsUpdates(mt gog_media.Media) (map[string]bool, error) {
	updates := make(map[string]bool, 0)
	vrAccountProducts, err := vangogh_values.NewReader(vangogh_products.AccountProducts, mt)
	if err != nil {
		return updates, err
	}

	for _, id := range vrAccountProducts.All() {
		ap, err := vrAccountProducts.AccountProduct(id)
		if err != nil {
			return updates, err
		}
		if ap.Updates > 0 {
			updates[id] = true
		}
	}

	return updates, nil
}

func itemizeUpdated(
	since int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) (map[string]bool, error) {

	updatedIds := make(map[string]bool, 0)

	//licence products can only update through creation and we've already handled
	//newly created in itemizeMissing func
	if pt == vangogh_products.LicenceProducts {
		return updatedIds, nil
	}

	mainDestUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return updatedIds, err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return updatedIds, err
	}

	for _, id := range kvMain.ModifiedAfter(since, false) {
		updatedIds[id] = true
	}

	return updatedIds, nil
}

func itemizeAPV2LinkedGames(modifiedAfter int64) (map[string]bool, error) {

	missing := make(map[string]bool, 0)

	//currently api-products-v2 support only gog_media.Game, and since this method is exclusively
	//using api-products-v2 we're fine specifying media directly and not taking as a parameter
	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)

	if err != nil {
		return missing, err
	}

	for _, id := range vrApv2.ModifiedAfter(modifiedAfter, false) {

		// have to use product reader and not extracts here, since extracts wouldn't be ready
		// while we're still getting data. Attempting to minimize the impact by only querying
		// new or updated api-product-v2 items since start to the sync
		apv2, err := vrApv2.ApiProductV2(id)

		if err != nil {
			return missing, err
		}

		linkedGames := apv2.GetIncludesGames()
		linkedGames = append(linkedGames, apv2.GetIsIncludedInGames()...)
		linkedGames = append(linkedGames, apv2.GetRequiresGames()...)
		linkedGames = append(linkedGames, apv2.GetIsRequiredByGames()...)

		for _, lid := range linkedGames {
			if !vrApv2.Contains(lid) {
				missing[lid] = true
			}
		}
	}

	return missing, nil
}

//itemizeRequiredGames enumerates all base products for a newly acquired DLCs
func itemizeRequiredGames(createdAfter int64, mt gog_media.Media) (map[string]bool, error) {
	reqGamesForNewLicences := make(map[string]bool, 0)

	vrLicences, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, mt)
	if err != nil {
		return nil, err
	}

	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)
	if err != nil {
		return nil, err
	}

	for _, id := range vrLicences.CreatedAfter(createdAfter) {
		// it's not guaranteed that a license would have an existing api-products-v2
		if !vrApv2.Contains(id) {
			continue
		}
		//like in itemizeMissingIncludesGames, we can't use extracts here,
		//because we're in process of getting data and would rather query api-products-v2 directly.
		//the performance impact is expected to be minimal since we're only loading newly acquired licences
		apv2, err := vrApv2.ApiProductV2(id)
		if err != nil {
			return nil, err
		}

		for _, rg := range apv2.GetRequiresGames() {
			reqGamesForNewLicences[rg] = true
		}
	}

	return reqGamesForNewLicences, nil
}
