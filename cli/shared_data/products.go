package shared_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"maps"
	"strconv"
)

func GetSteamGogIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.SteamAppIdProperty)
}

func GetPcgwGogIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.PcgwPageIdProperty)
}

func GetWikipediaIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.WikipediaIdProperty)
}

func GetOpenCriticGogIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.OpenCriticIdProperty)
}

func GetHltbIds(gogIds iter.Seq[string]) (map[string][]string, error) {
	return getExternalIdGogIds(gogIds, vangogh_integration.HltbIdProperty)
}

func getExternalIdGogIds(gogIds iter.Seq[string], externalIdProperty string) (map[string][]string, error) {
	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, externalIdProperty)
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

func GetCatalogAccountProducts(since int64) (map[string]any, error) {
	catalogAccountProductIds := make(map[string]any)

	catalogProductIds, err := GetCatalogPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for _, cpId := range catalogProductIds {
		catalogAccountProductIds[cpId] = nil
	}

	accountProductIds, err := GetAccountPagesProducts(since)
	if err != nil {
		return nil, err
	}

	for _, apId := range accountProductIds {
		catalogAccountProductIds[apId] = nil
	}

	return catalogAccountProductIds, nil
}

func GetGameGogIds(gogIds map[string]any) (map[string]any, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.ProductTypeProperty, vangogh_integration.IsDemoProperty)
	if err != nil {
		return nil, err
	}

	gameGogIds := make(map[string]any)

	for gogId := range gogIds {
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, gogId); ok && pt != vangogh_integration.GameProductType {
			continue
		}
		if demo, ok := rdx.GetLastVal(vangogh_integration.IsDemoProperty, gogId); ok && demo == vangogh_integration.TrueValue {
			continue
		}
		gameGogIds[gogId] = nil
	}

	return gameGogIds, nil
}

func AppendEditions(products map[string]any) (map[string]any, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.EditionsProperty)
	if err != nil {
		return nil, err
	}

	productsEditions := maps.Clone(products)

	for id := range products {
		if editions, ok := rdx.GetAllValues(vangogh_integration.EditionsProperty, id); ok {
			for _, eId := range editions {
				productsEditions[eId] = nil
			}
		}
	}

	return productsEditions, nil
}

func GetCatalogPagesProducts(since int64) ([]string, error) {

	gcppa := nod.NewProgress(" enumerating %s...", vangogh_integration.CatalogPage)
	defer gcppa.Done()

	catalogPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.CatalogPage)
	if err != nil {
		return nil, err
	}
	kvCatalogPages, err := kevlar.New(catalogPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	updatedPages := make([]string, 0)
	for page := range kvCatalogPages.Since(since, kevlar.Create, kevlar.Update) {
		updatedPages = append(updatedPages, page)
	}

	gcppa.TotalInt(len(updatedPages))

	catalogProductIds := make([]string, 0, kvCatalogPages.Len())

	for _, page := range updatedPages {
		ids, err := getCatalogPageProducts(page, kvCatalogPages)
		if err != nil {
			return nil, err
		}
		catalogProductIds = append(catalogProductIds, ids...)

		gcppa.Increment()
	}

	return catalogProductIds, nil
}

func getCatalogPageProducts(page string, kvCatalogPages kevlar.KeyValues) ([]string, error) {
	rcCatalogPage, err := kvCatalogPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.NewDecoder(rcCatalogPage).Decode(&catalogPage); err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(catalogPage.Products))

	for _, catalogProduct := range catalogPage.Products {
		ids = append(ids, catalogProduct.Id)
	}

	return ids, nil
}

func GetAccountPagesProducts(since int64) ([]string, error) {

	gappa := nod.NewProgress(" enumerating %s...", vangogh_integration.AccountPage)
	defer gappa.Done()

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return nil, err
	}
	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	updatedPages := make([]string, 0)
	for page := range kvAccountPages.Since(since, kevlar.Create, kevlar.Update) {
		updatedPages = append(updatedPages, page)
	}

	gappa.TotalInt(len(updatedPages))

	accountProductIds := make([]string, 0, kvAccountPages.Len())

	for _, page := range updatedPages {
		ids, err := getAccountPageProducts(page, kvAccountPages)
		if err != nil {
			return nil, err
		}
		accountProductIds = append(accountProductIds, ids...)

		gappa.Increment()
	}

	return accountProductIds, nil
}

func getAccountPageProducts(page string, kvAccountPages kevlar.KeyValues) ([]string, error) {
	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.NewDecoder(rcAccountPage).Decode(&accountPage); err != nil {
		return nil, err
	}

	ids := make([]string, 0, len(accountPage.Products))

	for _, accountProduct := range accountPage.Products {
		ids = append(ids, strconv.Itoa(accountProduct.Id))
	}

	return ids, nil
}

func GetDetailsUpdates(since int64) (map[string]any, error) {

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return nil, err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	newUpdatedDetails := make(map[string]any)

	newRequiredGameLicences, err := getNewRequiredGameLicences(kvDetails)
	if err != nil {
		return nil, err
	}

	for id := range newRequiredGameLicences {
		newUpdatedDetails[id] = nil
	}

	newUpdatedAccountProducts, err := getNewUpdatedAccountPages(kvDetails)
	if err != nil {
		return nil, err
	}

	for id := range newUpdatedAccountProducts {
		newUpdatedDetails[id] = nil
	}

	for id := range kvDetails.Since(since, kevlar.Create, kevlar.Update) {
		newUpdatedDetails[id] = nil
	}

	return newUpdatedDetails, nil
}

func getNewRequiredGameLicences(kvDetails kevlar.KeyValues) (iter.Seq[string], error) {

	gnla := nod.NewProgress(" enumerating %s updates...", vangogh_integration.Licences)
	defer gnla.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.LicencesProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.RequiresGamesProperty)
	if err != nil {
		return nil, err
	}

	return func(yield func(string) bool) {
		for licenceId := range rdx.Keys(vangogh_integration.LicencesProperty) {

			if productType, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, licenceId); ok {
				switch productType {
				case vangogh_integration.GameProductType:
					// do nothing
					continue
				case vangogh_integration.PackProductType:
					// skip, account products will have the corresponding GAME products
					continue
				case vangogh_integration.DlcProductType:
					// replace DLC licence Id with GAME product that is required for this DLC
					if requiresGame, sure := rdx.GetLastVal(vangogh_integration.RequiresGamesProperty, licenceId); sure {
						if !kvDetails.Has(requiresGame) && !yield(requiresGame) {
							return
						}
					}
				}
			}

		}
	}, nil
}

func getNewUpdatedAccountPages(kvDetails kevlar.KeyValues) (iter.Seq[string], error) {

	gnuapa := nod.NewProgress(" enumerating %s updates...", vangogh_integration.AccountPage)
	defer gnuapa.Done()

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return nil, err
	}

	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	newUpdatedAccountProducts := make(map[string]any)

	gnuapa.TotalInt(kvAccountPages.Len())

	for page := range kvAccountPages.Keys() {
		pageNewUpdatedAccountProducts, err := getPageNewUpdatedAccountProducts(page, kvAccountPages, kvDetails)
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

func getPageNewUpdatedAccountProducts(page string, kvAccountPages, kvDetails kevlar.KeyValues) (iter.Seq[string], error) {
	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.NewDecoder(rcAccountPage).Decode(&accountPage); err != nil {
		return nil, err
	}

	return func(yield func(string) bool) {
		for _, accountProduct := range accountPage.Products {
			id := strconv.Itoa(accountProduct.Id)
			if !kvDetails.Has(id) {
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
