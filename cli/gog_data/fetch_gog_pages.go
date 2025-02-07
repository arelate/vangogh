package gog_data

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
	"strconv"
)

const firstPageId = "1"

func getGogPages(pageUrlFunc idUrlFunc, hc *http.Client, kv kevlar.KeyValues, tpw nod.TotalProgressWriter, force bool) error {

	firstPageUrl := pageUrlFunc(firstPageId)
	if err := requestGogData(firstPageId, firstPageUrl, hc, http.MethodGet, kv); err != nil {
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

	pages := make([]string, 0, tpp.GetTotalPages()-1)
	for page := 2; page <= tpp.GetTotalPages(); page++ {
		pages = append(pages, strconv.Itoa(page))
	}

	if pageErrs := getGogItems(pageUrlFunc, hc, kv, tpw, pages...); len(pageErrs) > 0 {
		return fmt.Errorf("get pages errors: %v", pageErrs)
	}

	return nil
}
