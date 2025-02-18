package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"slices"
	"strconv"
)

const firstPageId = "1"

func fetchGogPages(pageReq *reqs.Builder, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, force bool) error {

	firstPageUrl := pageReq.UrlFunc(firstPageId)
	if err := fetch.SetValue(firstPageId, firstPageUrl, pageReq, kv); err != nil {
		return err
	}
	if tpw != nil {
		tpw.Increment()
	}

	rcFirstPage, err := kv.Get("1")
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

	pageErrs := fetch.Items(slices.Values(pages), pageReq, kv, tpw)

	if err = shared_data.WriteTypeErrors(pageReq.ProductType, pageErrs); err != nil {
		return err
	}

	return nil
}
