package gog_data

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
	"strconv"
)

const firstPageId = "1"

func fetchGogPages(pageUrlFunc fetch.IdUrlFunc, hc *http.Client, method string, authBearer string, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, force bool) error {

	firstPageUrl := pageUrlFunc(firstPageId)
	if err := fetch.SetValue(firstPageId, firstPageUrl, hc, method, authBearer, kv); err != nil {
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

	if pageErrs := fetch.Items(pageUrlFunc, hc, method, authBearer, kv, tpw, pages...); len(pageErrs) > 0 {
		return fmt.Errorf("get pages errors: %v", pageErrs)
	}

	return nil
}
