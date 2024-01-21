package vets

import (
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"strconv"
)

func findLocalOnlySplitProducts(pagedPt vangogh_local_data.ProductType) (map[string]bool, error) {

	idSet := make(map[string]bool)

	if !vangogh_local_data.IsGOGPagedProduct(pagedPt) {
		return idSet, fmt.Errorf("%s is not a paged type", pagedPt)
	}

	pagedIds := make(map[string]bool)

	vrPaged, err := vangogh_local_data.NewProductReader(pagedPt)
	if err != nil {
		return idSet, err
	}

	for _, id := range vrPaged.Keys() {
		productGetter, err := vrPaged.ProductsGetter(id)
		if err != nil {
			return idSet, err
		}
		for _, idGetter := range productGetter.GetProducts() {
			pid := strconv.Itoa(idGetter.GetId())
			pagedIds[pid] = true
		}
	}

	splitPt := vangogh_local_data.SplitProductType(pagedPt)
	vrSplit, err := vangogh_local_data.NewProductReader(splitPt)
	if err != nil {
		return idSet, err
	}

	splitIdSet := make(map[string]bool)
	for _, sid := range vrSplit.Keys() {
		splitIdSet[sid] = true
	}

	localOnly := make(map[string]bool)

	for id := range splitIdSet {
		if !pagedIds[id] {
			localOnly[id] = true
		}
	}

	return localOnly, nil
}
