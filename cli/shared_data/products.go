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
	"strconv"
)

func GetSteamGogIds(gogIds iter.Seq[string]) (map[string]string, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.SteamAppIdProperty)
	if err != nil {
		return nil, err
	}

	steamGogIds := make(map[string]string)

	for gogId := range gogIds {
		if steamAppId, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok && steamAppId != "" {
			steamGogIds[steamAppId] = gogId
		}
	}

	return steamGogIds, nil
}

func GetPcgwGogIds(gogIds iter.Seq[string]) (map[string]string, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.PcgwPageIdProperty)
	if err != nil {
		return nil, err
	}

	pcgwGogIds := make(map[string]string)

	for gogId := range gogIds {
		if pcgwPageId, ok := rdx.GetLastVal(vangogh_integration.PcgwPageIdProperty, gogId); ok && pcgwPageId != "" {
			pcgwGogIds[pcgwPageId] = gogId
		}
	}

	return pcgwGogIds, nil
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
