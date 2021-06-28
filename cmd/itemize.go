package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/kvas"
)

func itemizeAll(
	ids []string,
	slug string,
	missing, updated bool,
	modifiedAfter int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) ([]string, error) {

	idSet := gost.StrSetWith(ids...)

	if slug != "" {
		exl, err := vangogh_extracts.NewList(vangogh_properties.SlugProperty)
		if err != nil {
			return nil, err
		}
		slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
		idSet.Add(slugIds...)
	}

	for _, mainPt := range vangogh_products.MainTypes(pt) {
		if missing {
			missingIds, err := itemizeMissing(pt, mainPt, mt, modifiedAfter)
			if err != nil {
				return idSet.All(), err
			}
			if len(missingIds) == 0 {
				fmt.Printf(" no missing %s for %s (%s)\n", pt, mainPt, mt)
			}
			idSet.Add(missingIds...)
		}
		if updated {
			updatedIds, err := itemizeUpdated(modifiedAfter, mainPt, mt)
			if err != nil {
				return ids, err
			}
			if len(updatedIds) == 0 {
				fmt.Printf(" no updated %s for %s (%s)\n", pt, mainPt, mt)
			}
			idSet.Add(updatedIds...)
		}
	}

	return idSet.All(), nil
}

func itemizeMissing(
	detailPt, mainPt vangogh_products.ProductType,
	mt gog_media.Media,
	modifiedAfter int64) ([]string, error) {

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
		fmt.Printf("checking missing required base %s for %s\n", detailPt, mainPt)
		return itemizeRequiredGames(modifiedAfter, mt)
	}

	fmt.Printf("checking missing %s for %s\n", detailPt, mainPt)

	missingIdSet := gost.NewStrSet()

	mainDestUrl, err := vangogh_urls.LocalProductsDir(mainPt, mt)
	if err != nil {
		return missingIdSet.All(), err
	}

	detailDestUrl, err := vangogh_urls.LocalProductsDir(detailPt, mt)
	if err != nil {
		return missingIdSet.All(), err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return missingIdSet.All(), err
	}

	kvDetail, err := kvas.NewJsonLocal(detailDestUrl)
	if err != nil {
		return missingIdSet.All(), err
	}
	for _, id := range kvMain.All() {
		if !kvDetail.Contains(id) {
			missingIdSet.Add(id)
		}
	}

	if mainPt == vangogh_products.AccountProducts &&
		detailPt == vangogh_products.Details {
		fmt.Printf("checking %s updates to update %s\n", mainPt, detailPt)
		updatedAccountProducts, err := itemizeAccountProductsUpdates(mt)
		if err != nil {
			return missingIdSet.All(), err
		}
		for _, uapId := range updatedAccountProducts {
			missingIdSet.Add(uapId)
		}
	}

	return missingIdSet.All(), nil
}

func itemizeAccountProductsUpdates(mt gog_media.Media) ([]string, error) {
	updatesSet := gost.NewStrSet()
	vrAccountProducts, err := vangogh_values.NewReader(vangogh_products.AccountProducts, mt)
	if err != nil {
		return updatesSet.All(), err
	}

	for _, id := range vrAccountProducts.All() {
		ap, err := vrAccountProducts.AccountProduct(id)
		if err != nil {
			return updatesSet.All(), err
		}
		if ap.Updates > 0 {
			updatesSet.Add(id)
		}
	}

	return updatesSet.All(), nil
}

func itemizeUpdated(
	since int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) ([]string, error) {

	updatedIdSet := gost.NewStrSet()

	//licence products can only update through creation and we've already handled
	//newly created in itemizeMissing func
	if pt == vangogh_products.LicenceProducts {
		return updatedIdSet.All(), nil
	}

	mainDestUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return updatedIdSet.All(), err
	}

	kvMain, err := kvas.NewJsonLocal(mainDestUrl)
	if err != nil {
		return updatedIdSet.All(), err
	}

	for _, id := range kvMain.ModifiedAfter(since, false) {
		updatedIdSet.Add(id)
	}

	return updatedIdSet.All(), nil
}

func itemizeAPV2LinkedGames(modifiedAfter int64) ([]string, error) {

	missingSet := gost.NewStrSet()

	//currently api-products-v2 support only gog_media.Game, and since this method is exclusively
	//using api-products-v2 we're fine specifying media directly and not taking as a parameter
	vrApv2, err := vangogh_values.NewReader(vangogh_products.ApiProductsV2, gog_media.Game)

	if err != nil {
		return missingSet.All(), err
	}

	for _, id := range vrApv2.ModifiedAfter(modifiedAfter, false) {

		// have to use product reader and not extracts here, since extracts wouldn't be ready
		// while we're still getting data. Attempting to minimize the impact by only querying
		// new or updated api-product-v2 items since start to the sync
		apv2, err := vrApv2.ApiProductV2(id)

		if err != nil {
			return missingSet.All(), err
		}

		linkedGames := apv2.GetIncludesGames()
		linkedGames = append(linkedGames, apv2.GetIsIncludedInGames()...)
		linkedGames = append(linkedGames, apv2.GetRequiresGames()...)
		linkedGames = append(linkedGames, apv2.GetIsRequiredByGames()...)

		for _, lid := range linkedGames {
			if !vrApv2.Contains(lid) {
				missingSet.Add(lid)
			}
		}
	}

	return missingSet.All(), nil
}

//itemizeRequiredGames enumerates all base products for a newly acquired DLCs
func itemizeRequiredGames(createdAfter int64, mt gog_media.Media) ([]string, error) {
	rgForNewLicSet := gost.NewStrSet()

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

		for _, reqGame := range apv2.GetRequiresGames() {
			rgForNewLicSet.Add(reqGame)
		}
	}

	return rgForNewLicSet.All(), nil
}
