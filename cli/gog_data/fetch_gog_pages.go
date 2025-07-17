package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"slices"
	"strconv"
	"time"
)

const firstPageId = "1"

func fetchGogPages(pageReq *reqs.Params, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, force bool) error {

	firstPageUrl := pageReq.UrlFunc(firstPageId)

	ptId, err := vangogh_integration.ProductTypeId(pageReq.ProductType, firstPageId)
	if err != nil {
		return err
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	if err = fetch.RequestSetValue(firstPageId, firstPageUrl, pageReq, kv); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)
		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		// the error fetching the first page has been saved, there's nothing else to do here
		// returning to continue original operation uninterrupted
		return nil
	}
	if tpw != nil {
		tpw.Increment()
	}

	rcFirstPage, err := kv.Get(firstPageId)
	if err != nil {
		return err
	}
	defer rcFirstPage.Close()

	var tpp gog_integration.TotalPagesProxy
	if err = json.NewDecoder(rcFirstPage).Decode(&tpp); err != nil {
		return err
	}

	if tpp.GetTotalPages() == kv.Len() && !force {
		return nil
	}

	pages := make([]string, 0, tpp.GetTotalPages())
	for page := 2; page <= tpp.GetTotalPages(); page++ {
		pages = append(pages, strconv.Itoa(page))
	}

	if tpw != nil {
		tpw.TotalInt(tpp.GetTotalPages())
	}

	if err = fetch.Items(slices.Values(pages), pageReq, kv, tpw, force); err != nil {
		return err
	}

	return nil
}
