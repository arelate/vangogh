package gog_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"maps"
	"net/http"
)

func GetRelatedApiProducts(hc *http.Client, uat string, since int64) error {

	grapva := nod.NewProgress("getting related %s...", vangogh_integration.ApiProducts)
	defer grapva.Done()

	apiProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ApiProducts)
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

	grapva.TotalInt(len(nrIds))

	if err = fetch.Items(maps.Keys(nrIds), reqs.ApiProducts(hc, uat), kvApiProducts, grapva); err != nil {
		return err
	}

	return ReduceApiProducts(kvApiProducts, since)
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
