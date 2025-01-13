package vets

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"strconv"
)

func findLocalOnlySplitProducts(pagedPt vangogh_integration.ProductType) (map[string]bool, error) {

	idSet := make(map[string]bool)

	if !vangogh_integration.IsGOGPagedProduct(pagedPt) {
		return idSet, fmt.Errorf("%s is not a paged type", pagedPt)
	}

	pagedIds := make(map[string]bool)

	vrPaged, err := vangogh_integration.NewProductReader(pagedPt)
	if err != nil {
		return nil, err
	}

	keys, err := vrPaged.Keys()
	if err != nil {
		return nil, err
	}
	for _, id := range keys {
		productGetter, err := vrPaged.ProductsGetter(id)
		if err != nil {
			return idSet, err
		}
		for _, idGetter := range productGetter.GetProducts() {
			pid := strconv.Itoa(idGetter.GetId())
			pagedIds[pid] = true
		}
	}

	splitPt := vangogh_integration.SplitProductType(pagedPt)
	vrSplit, err := vangogh_integration.NewProductReader(splitPt)
	if err != nil {
		return nil, err
	}

	splitIdSet := make(map[string]bool)
	keys, err = vrSplit.Keys()
	if err != nil {
		return nil, err
	}
	for _, sid := range keys {
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
