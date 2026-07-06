package gog_data

import (
	"iter"
	"maps"
	"net/http"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetRelatedGogApiProducts(hc *http.Client, uat string, since int64, force bool) error {

	grapva := nod.NewProgress("getting related %s...", vangogh_integration.GogApiProducts)
	defer grapva.Done()

	gogApiProductsDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogApiProducts)

	kvGogApiProducts, err := kevlar.New(gogApiProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	relatedApiProductProperties := []string{
		vangogh_integration.GogRequiresGamesProperty,
		vangogh_integration.GogIsRequiredByGamesProperty,
		vangogh_integration.GogIncludesGamesProperty,
		vangogh_integration.GogIsIncludedByGamesProperty,
	}

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), relatedApiProductProperties...)
	if err != nil {
		return err
	}

	nrIds := make(map[string]any)

	for id := range kvGogApiProducts.Since(since, kevlar.Create, kevlar.Update) {
		for nid := range relatedProducts(id, rdx, vangogh_integration.GogRequiresGamesProperty) {
			nrIds[nid] = nil
		}
		for nid := range relatedProducts(id, rdx, vangogh_integration.GogIsRequiredByGamesProperty) {
			nrIds[nid] = nil
		}
		for nid := range relatedProducts(id, rdx, vangogh_integration.GogIncludesGamesProperty) {
			nrIds[nid] = nil
		}
		for nid := range relatedProducts(id, rdx, vangogh_integration.GogIsIncludedByGamesProperty) {
			nrIds[nid] = nil
		}
	}

	grapva.TotalInt(len(nrIds))

	if err = fetch.Items(maps.Keys(nrIds), reqs.GogApiProducts(hc, uat), kvGogApiProducts, grapva, force); err != nil {
		return err
	}

	return ReduceGogApiProducts(kvGogApiProducts, since, force)
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
