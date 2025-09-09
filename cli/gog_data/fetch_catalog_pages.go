package gog_data

import (
	"encoding/json"
	"strconv"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func fetchCatalogPages(catalogPageReq *reqs.Params, kvCatalogPages kevlar.KeyValues, tpw nod.TotalProgressWriter) error {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	currentCatalogPage := 1
	lastExternalProductId := ""
	productCount := 0

	for {
		if err = fetchCatalogPage(currentCatalogPage, lastExternalProductId, catalogPageReq, kvCatalogPages, rdx); err != nil {
			return err
		}

		if productCount == 0 {
			productCount, err = getProductCount(currentCatalogPage, kvCatalogPages)
			if err != nil {
				return err
			}

			if productCount > 0 && tpw != nil {
				// 12345 / 100 = 123
				// + one page for the remaining 45 products (12345 - 12300)
				// + the last resulting page with empty products list
				tpw.TotalInt((productCount / gog_integration.CatalogPagesProductsLimit) + 1 + 1)
			}
		}

		if tpw != nil {
			tpw.Increment()
		}

		lastExternalProductId, err = getLastExternalProductId(currentCatalogPage, kvCatalogPages)
		if err != nil {
			return err
		}

		if lastExternalProductId == "" {
			break
		}

		currentCatalogPage++
	}

	return nil
}

func fetchCatalogPage(currentPage int, searchAfter string, pageReq *reqs.Params, kvCatalogPages kevlar.KeyValues, rdx redux.Writeable) error {
	catalogPageUrl := pageReq.UrlFunc(searchAfter)

	ptId, err := vangogh_integration.ProductTypeId(pageReq.ProductType, strconv.Itoa(currentPage))
	if err != nil {
		return err
	}

	formattedNow := time.Now().UTC().Format(time.RFC3339)

	if err = fetch.RequestSetValue(strconv.Itoa(currentPage), catalogPageUrl, pageReq, kvCatalogPages); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		// the error fetching the first page has been saved, there's nothing else to do here
		// returning to continue original operation uninterrupted
		return nil
	}

	return rdx.ReplaceValues(vangogh_integration.GetDataLastUpdatedProperty, ptId, formattedNow)
}

func getLastExternalProductId(currentPage int, kvCatalogPage kevlar.KeyValues) (string, error) {

	rcCatalogPage, err := kvCatalogPage.Get(strconv.Itoa(currentPage))
	if err != nil {
		return "", err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.NewDecoder(rcCatalogPage).Decode(&catalogPage); err != nil {
		return "", err
	}

	switch len(catalogPage.Products) {
	case 0:
		return "", nil
	default:
		return catalogPage.Products[len(catalogPage.Products)-1].Id, nil
	}
}

func getProductCount(currentPage int, kvCatalogPage kevlar.KeyValues) (int, error) {

	rcCatalogPage, err := kvCatalogPage.Get(strconv.Itoa(currentPage))
	if err != nil {
		return -1, err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.NewDecoder(rcCatalogPage).Decode(&catalogPage); err != nil {
		return -1, err
	}

	return catalogPage.ProductCount, nil
}
