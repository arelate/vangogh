package itemize

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/kvas"
)

func missingDetail(
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	since int64) (gost.StrSet, error) {

	//api-products-v2 provides
	//includes-games, is-included-by-games,
	//requires-games, is-required-by-games
	if mainPt == vangogh_products.ApiProductsV2 &&
		detailPt == vangogh_products.ApiProductsV2 {
		fmt.Printf(" finding missing linked %s... ", vangogh_products.ApiProductsV2)
		lg, err := linkedGames(since)
		if err != nil {
			return lg, err
		}
		printFoundAndAll(lg)
		return lg, nil
	}

	//licences give a signal when DLC has been purchased, this would add
	//required (base) game details to the updates
	if mainPt == vangogh_products.LicenceProducts &&
		detailPt == vangogh_products.Details {
		fmt.Printf(" finding DLCs missing required base product... ")
		rg, err := RequiredAndIncluded(since)
		if err != nil {
			return rg, err
		}
		printFoundAndAll(rg)
		return rg, nil
	}

	fmt.Printf(" finding missing %s for %s... ", detailPt, mainPt)

	missingIdSet := gost.NewStrSet()

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return missingIdSet, err
	}

	detailDestUrl, err := vangogh_urls.LocalProductsDir(detailPt, mt)
	if err != nil {
		return missingIdSet, err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return missingIdSet, err
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
	if err != nil {
		return missingIdSet, err
	}

	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) {
			missingIdSet.Add(id)
		}
	}

	printFoundAndAll(missingIdSet)

	if mainPt == vangogh_products.AccountProducts &&
		detailPt == vangogh_products.Details {
		fmt.Printf(" finding %s updates... ", vangogh_products.AccountProducts)
		updatedAccountProducts, err := AccountProductsUpdates(mt)
		if err != nil {
			return missingIdSet, err
		}
		printFoundAndAll(updatedAccountProducts)
		for uapId := range updatedAccountProducts {
			missingIdSet.Add(uapId)
		}
	}

	return missingIdSet, nil
}
