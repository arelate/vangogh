package gog_data

import (
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"maps"
	"net/http"
	"slices"
)

func GetRelatedApiProducts(since int64, force bool) error {

	grapva := nod.NewProgress("getting related %s...", vangogh_integration.ApiProductsV2)
	defer grapva.Done()

	if force {
		since = -1
	}

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProductsV2)
	if err != nil {
		return err
	}
	kvApiProducts, err := kevlar.New(apiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	relatedApiProductProperties := []string{
		vangogh_integration.RequiresGamesProperty,
		vangogh_integration.IsRequiredByGamesProperty,
		vangogh_integration.IncludesGamesProperty,
		vangogh_integration.IsIncludedByGamesProperty,
	}

	rdx, err := redux.NewWriter(reduxDir, relatedApiProductProperties...)
	if err != nil {
		return err
	}

	nrIds := make(map[string]any)

	for id := range kvApiProducts.Since(since, kevlar.Create, kevlar.Update) {
		for nid := range relatedProducts(id, rdx, vangogh_integration.RequiresGamesProperty) {
			nrIds[nid] = nil
		}
		for nid := range relatedProducts(id, rdx, vangogh_integration.IsRequiredByGamesProperty) {
			nrIds[nid] = nil
		}
		for nid := range relatedProducts(id, rdx, vangogh_integration.IncludesGamesProperty) {
			nrIds[nid] = nil
		}
		for nid := range relatedProducts(id, rdx, vangogh_integration.IsIncludedByGamesProperty) {
			nrIds[nid] = nil
		}
	}

	relatedIds := slices.Collect(maps.Keys(nrIds))
	if itemErrs := getGogItems(gog_integration.ApiProductV2Url, http.DefaultClient, kvApiProducts, grapva, relatedIds...); len(itemErrs) > 0 {
		return fmt.Errorf("get %s errors: %v", vangogh_integration.ApiProductsV2, itemErrs)
	}

	return reduceApiProducts(kvApiProducts, relatedIds...)
}

func relatedProducts(id string, rdx redux.Readable, property string) iter.Seq[string] {
	newIds := make(map[string]any)
	if relatedGames, ok := rdx.GetAllValues(property, id); ok {
		for _, relatedGameId := range relatedGames {
			newIds[relatedGameId] = nil
		}
	}
	return maps.Keys(newIds)
}
