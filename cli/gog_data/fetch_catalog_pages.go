package gog_data

import (
	"encoding/json/v2"
	"strconv"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func fetchCatalogPages(catalogPageReq *reqs.Params, kvCatalogPages kevlar.KeyValues, tpw nod.TotalProgressWriter) error {

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	currentCatalogPage := 1
	lastExternalProductId := ""
	productCount, totalPages := 0, 0

	for {
		if err = fetchCatalogPage(currentCatalogPage, lastExternalProductId, catalogPageReq, kvCatalogPages, rdx); err != nil {
			return err
		}

		productCount, lastExternalProductId, err = getProductCountLastId(currentCatalogPage, kvCatalogPages)
		if err != nil {
			return err
		}

		if totalPages == 0 && productCount > 0 && tpw != nil {
			// 12345 / 100 = 123
			// + one page for the remaining 45 products (12345 - 12300)
			// + the last resulting page with empty products list
			totalPages = productCount / gog_integration.CatalogPagesProductsLimit
			if totalPages*gog_integration.CatalogPagesProductsLimit < productCount {
				totalPages++
			}
			totalPages++
			tpw.TotalInt(totalPages)
		}

		if lastExternalProductId == "" {
			break
		}

		if tpw != nil {
			tpw.Increment()
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

func getProductCountLastId(currentPage int, kvCatalogPage kevlar.KeyValues) (int, string, error) {

	rcCatalogPage, err := kvCatalogPage.Get(strconv.Itoa(currentPage))
	if err != nil {
		return -1, "", err
	}
	defer rcCatalogPage.Close()

	var catalogPage gog_integration.CatalogPage
	if err = json.UnmarshalRead(rcCatalogPage, &catalogPage); err != nil {
		return -1, "", err
	}

	switch len(catalogPage.Products) {
	case 0:
		return -1, "", nil
	default:
		return catalogPage.ProductCount, catalogPage.Products[len(catalogPage.Products)-1].Id, nil
	}
}
