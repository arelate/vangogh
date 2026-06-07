package shared_data

import (
	"encoding/json/v2"
	"iter"
	"maps"
	"strconv"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetSteamGogIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.GogSteamAppIdProperty)
}

func GetPcgwGogIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.GogPcgwPageIdProperty)
}

func GetWikipediaIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.GogWikipediaIdProperty)
}

func GetOpenCriticGogIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.GogOpenCriticIdProperty)
}

func GetHltbIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.GogHltbIdProperty)
}

func getExternalIdGogIds(gogIds iter.Seq[string], externalIdProperty string) (map[string][]string, error) {

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(), externalIdProperty)
	if err != nil {
		return nil, err
	}

	externalIdGogIds := make(map[string][]string)

	for gogId := range gogIds {
		if externalIds, ok := rdx.GetAllValues(externalIdProperty, gogId); ok && len(externalIds) > 0 {
			for _, externalId := range externalIds {
				if externalId == "" {
					continue
				}
				externalIdGogIds[externalId] = append(externalIdGogIds[externalId], gogId)
			}
		}
	}

	return externalIdGogIds, nil
}

func GetGogCatalogAccountProducts(since int64) (map[string]any, error) {
	gogCatalogAccountProductIds := make(map[string]any)

	gogCatalogProductIds, err := GetGogCatalogPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for _, cpId := range gogCatalogProductIds {
		gogCatalogAccountProductIds[cpId] = nil
	}

	gogAccountProductIds, err := GetGogAccountPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for _, apId := range gogAccountProductIds {
		gogCatalogAccountProductIds[apId] = nil
	}

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogRootEditionsProperty,
		vangogh_integration.GogIncludesGamesProperty)
	if err != nil {
		return nil, err
	}

	for id := range gogCatalogAccountProductIds {
		if rootEditionIds, ok := rdx.GetAllValues(vangogh_integration.GogRootEditionsProperty, id); ok {
			for _, reId := range rootEditionIds {
				gogCatalogAccountProductIds[reId] = nil
			}
		}
		if includesGamesIds, ok := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, id); ok {
			for _, igId := range includesGamesIds {
				gogCatalogAccountProductIds[igId] = nil
			}
		}
	}

	return gogCatalogAccountProductIds, nil
}

func GetGameGogIds(gogIds map[string]any) (map[string]any, error) {

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogProductTypeProperty,
		vangogh_integration.GogIsDemoProperty)
	if err != nil {
		return nil, err
	}

	gameGogIds := make(map[string]any)

	for gogId := range gogIds {
		if pt, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, gogId); ok && pt != gog_integration.ProductTypeGame {
			continue
		}
		if demo, ok := rdx.GetLastVal(vangogh_integration.GogIsDemoProperty, gogId); ok && demo == vangogh_integration.TrueValue {
			continue
		}
		gameGogIds[gogId] = nil
	}

	return gameGogIds, nil
}

func AppendGogEditions(gogProducts map[string]any) (map[string]any, error) {

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(), vangogh_integration.GogEditionsProperty)
	if err != nil {
		return nil, err
	}

	productsEditions := maps.Clone(gogProducts)

	for id := range gogProducts {
		if editions, ok := rdx.GetAllValues(vangogh_integration.GogEditionsProperty, id); ok {
			for _, eId := range editions {
				productsEditions[eId] = nil
			}
		}
	}

	return productsEditions, nil
}

func GetGogCatalogPagesProducts(since int64) ([]string, error) {

	gcppa := nod.NewProgress(" enumerating %s...", vangogh_integration.GogCatalogPage)
	defer gcppa.Done()

	gogCatalogPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogCatalogPage)
	if err != nil {
		return nil, err
	}
	kvGogCatalogPages, err := kevlar.New(gogCatalogPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	updatedPages := make([]string, 0)
	for page := range kvGogCatalogPages.Since(since, kevlar.Create, kevlar.Update) {
		updatedPages = append(updatedPages, page)
	}

	gcppa.TotalInt(len(updatedPages))

	gogCatalogProductIds := make([]string, 0)

	for _, page := range updatedPages {
		var ids []string
		ids, err = getGogCatalogPageProducts(page, kvGogCatalogPages)
		if err != nil {
			return nil, err
		}
		gogCatalogProductIds = append(gogCatalogProductIds, ids...)

		gcppa.Increment()
	}

	return gogCatalogProductIds, nil
}

func getGogCatalogPageProducts(page string, kvGogCatalogPages kevlar.KeyValues) ([]string, error) {
	rcCatalogPage, err := kvGogCatalogPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.UnmarshalRead(rcCatalogPage, &catalogPage); err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(catalogPage.Products))

	for _, catalogProduct := range catalogPage.Products {
		ids = append(ids, catalogProduct.Id)
	}

	return ids, nil
}

func GetGogAccountPagesProducts(since int64) ([]string, error) {

	gappa := nod.NewProgress(" enumerating %s...", vangogh_integration.GogAccountPage)
	defer gappa.Done()

	gogAccountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogAccountPage)
	if err != nil {
		return nil, err
	}
	kvGogAccountPages, err := kevlar.New(gogAccountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	updatedPages := make([]string, 0)
	for page := range kvGogAccountPages.Since(since, kevlar.Create, kevlar.Update) {
		updatedPages = append(updatedPages, page)
	}

	gappa.TotalInt(len(updatedPages))

	accountProductIds := make([]string, 0)

	for _, page := range updatedPages {
		var ids []string
		ids, err = getGogAccountPageProducts(page, kvGogAccountPages)
		if err != nil {
			return nil, err
		}
		accountProductIds = append(accountProductIds, ids...)

		gappa.Increment()
	}

	return accountProductIds, nil
}

func getGogAccountPageProducts(page string, kvGogAccountPages kevlar.KeyValues) ([]string, error) {
	rcGogAccountPage, err := kvGogAccountPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcGogAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.UnmarshalRead(rcGogAccountPage, &accountPage); err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(accountPage.Products))

	for _, accountProduct := range accountPage.Products {
		ids = append(ids, strconv.Itoa(accountProduct.Id))
	}

	return ids, nil
}

func GetGogDetailsUpdates(since int64) (map[string]any, error) {

	gogDetailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogDetails)
	if err != nil {
		return nil, err
	}

	kvGogDetails, err := kevlar.New(gogDetailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	newUpdatedDetails := make(map[string]any)

	newUpdatedOrderPagesProducts, err := getNewUpdatedGogOrderPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for id := range newUpdatedOrderPagesProducts {
		newUpdatedDetails[id] = nil
	}

	newUpdatedAccountProducts, err := getNewUpdatedGogAccountPages(kvGogDetails)
	if err != nil {
		return nil, err
	}

	for id := range newUpdatedAccountProducts {
		newUpdatedDetails[id] = nil
	}

	for id := range kvGogDetails.Since(since, kevlar.Create, kevlar.Update) {
		newUpdatedDetails[id] = nil
	}

	return newUpdatedDetails, nil
}

func getNewUpdatedGogOrderPagesProducts(since int64) (iter.Seq[string], error) {

	gopa := nod.NewProgress(" enumerating %s new, updated products...", vangogh_integration.GogOrderPage)
	defer gopa.Done()

	gogOrderPageDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogOrderPage)
	if err != nil {
		return nil, err
	}

	kvGogOrderPages, err := kevlar.New(gogOrderPageDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogProductTypeProperty,
		vangogh_integration.GogRequiresGamesProperty,
		vangogh_integration.GogIncludesGamesProperty)

	orderPagesProducts := make(map[string]any)

	for opId := range kvGogOrderPages.Since(since, kevlar.Create, kevlar.Update) {

		var orderPageProductsIds iter.Seq[string]
		orderPageProductsIds, err = getGogOrderPageProducts(opId, since, kvGogOrderPages)
		if err != nil {
			return nil, err
		}

		var orderPageIncludesRequires map[string]any
		orderPageIncludesRequires, err = getGogOrderProductsIncludesRequires(orderPageProductsIds, rdx)
		if err != nil {
			return nil, err
		}

		for includesRequiresId := range orderPageIncludesRequires {
			orderPagesProducts[includesRequiresId] = nil
		}

	}

	return maps.Keys(orderPagesProducts), nil
}

func getGogOrderPageProducts(id string, since int64, kvGogOrderPage kevlar.KeyValues) (iter.Seq[string], error) {
	rcGogOrderPage, err := kvGogOrderPage.Get(id)
	if err != nil {
		return nil, err
	}

	defer rcGogOrderPage.Close()

	var orderPage gog_integration.OrderPage

	if err = json.UnmarshalRead(rcGogOrderPage, &orderPage); err != nil {
		return nil, err
	}

	return func(yield func(string) bool) {
		for _, order := range orderPage.Orders {
			if order.Date < since {
				continue
			}
			for _, op := range order.Products {
				if !yield(op.Id) {
					return
				}
			}
		}
	}, nil

}

func getGogOrderProductsIncludesRequires(gogOrderPageProductsIds iter.Seq[string], rdx redux.Readable) (map[string]any, error) {

	if err := rdx.MustHave(vangogh_integration.GogIncludesGamesProperty, vangogh_integration.GogRequiresGamesProperty); err != nil {
		return nil, err
	}

	orderedGames := make(map[string]any)

	for productId := range gogOrderPageProductsIds {

		if productType, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, productId); ok {

			switch productType {
			case gog_integration.ProductTypeGame:
				orderedGames[productId] = nil
			case gog_integration.ProductTypePack:
				if includesIds, sure := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, productId); sure {
					for _, id := range includesIds {
						if pt, yeah := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, id); yeah && pt == gog_integration.ProductTypeGame {
							orderedGames[id] = nil
						}
					}
				}
			case gog_integration.ProductTypeDlc:
				if requiresIds, sure := rdx.GetAllValues(vangogh_integration.GogRequiresGamesProperty, productId); sure {
					for _, id := range requiresIds {
						orderedGames[id] = nil
					}
				}
			}
		}
	}

	return orderedGames, nil
}

func getNewUpdatedGogAccountPages(kvDetails kevlar.KeyValues) (iter.Seq[string], error) {

	gnuapa := nod.NewProgress(" enumerating %s updates...", vangogh_integration.GogAccountPage)
	defer gnuapa.Done()

	gogAccountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogAccountPage)
	if err != nil {
		return nil, err
	}

	kvGogAccountPages, err := kevlar.New(gogAccountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	newUpdatedAccountProducts := make(map[string]any)

	gnuapa.TotalInt(kvGogAccountPages.Len())

	for page := range kvGogAccountPages.Keys() {
		pageNewUpdatedAccountProducts, err := getPageNewUpdatedGogAccountProducts(page, kvGogAccountPages, kvDetails)
		if err != nil {
			return nil, err
		}
		for id := range pageNewUpdatedAccountProducts {
			newUpdatedAccountProducts[id] = nil
		}
		gnuapa.Increment()
	}

	if len(newUpdatedAccountProducts) == 0 {
		gnuapa.EndWithResult("no updates found")
	} else {
		gnuapa.EndWithResult("found %d updates", len(newUpdatedAccountProducts))
	}

	return maps.Keys(newUpdatedAccountProducts), nil
}

func getPageNewUpdatedGogAccountProducts(page string, kvGogAccountPages, kvGogDetails kevlar.KeyValues) (iter.Seq[string], error) {
	rcGogAccountPage, err := kvGogAccountPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcGogAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.UnmarshalRead(rcGogAccountPage, &accountPage); err != nil {
		return nil, err
	}

	return func(yield func(string) bool) {
		for _, accountProduct := range accountPage.Products {
			id := strconv.Itoa(accountProduct.Id)
			if !kvGogDetails.Has(id) {
				if !yield(id) {
					return
				}
			}
			if accountProduct.IsNew || accountProduct.Updates > 0 {
				if !yield(id) {
					return
				}
			}
		}
	}, nil
}
