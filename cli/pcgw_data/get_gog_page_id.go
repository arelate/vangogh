package pcgw_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
)

func GetGogPageId(gogIds map[string]any, force bool) error {

	gspia := nod.NewProgress("getting %s...", vangogh_integration.PcgwGogPageId)
	defer gspia.Done()

	gogPageIdDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwGogPageId)
	if err != nil {
		return err
	}

	kvGogPageId, err := kevlar.New(gogPageIdDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gameGogIds, err := getGameGogIds(gogIds)
	if err != nil {
		return err
	}

	newGameGogIds := getNewGogIds(gameGogIds, kvGogPageId, force)

	gspia.TotalInt(len(newGameGogIds))

	if err = fetch.Items(maps.Keys(newGameGogIds), reqs.PcgwGogPageId(), kvGogPageId, gspia); err != nil {
		return err
	}

	return reduceGogPageIds(newGameGogIds, kvGogPageId)
}

func getNewGogIds(gogIds map[string]any, kv kevlar.KeyValues, force bool) map[string]any {

	if force {
		return gogIds
	}

	newGogIds := make(map[string]any, len(gogIds))

	for gogId := range gogIds {
		if kv.Has(gogId) {
			continue
		}
		newGogIds[gogId] = nil
	}

	return newGogIds
}

func getGameGogIds(gogIds map[string]any) (map[string]any, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.PcgwPageIdProperties()...)
	if err != nil {
		return nil, err
	}

	gameGogIds := make(map[string]any)

	for gogId := range gogIds {
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, gogId); ok && pt == "GAME" {
			gameGogIds[gogId] = nil
		}
	}

	return gameGogIds, nil
}

func reduceGogPageIds(gogIds map[string]any, kvPageId kevlar.KeyValues) error {

	rpia := nod.Begin(" reducing %s...", vangogh_integration.PcgwGogPageId)
	defer rpia.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.PcgwPageIdProperties()...)
	if err != nil {
		return err
	}

	pageIdReductions := shared_data.InitReductions(vangogh_integration.PcgwPageIdProperties()...)

	for gogId := range gogIds {
		if err = reduceGogPageIdProduct(gogId, kvPageId, pageIdReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, pageIdReductions)
}

func reduceGogPageIdProduct(gogId string, kvPageId kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcPageId, err := kvPageId.Get(gogId)
	if err != nil {
		return err
	}
	defer rcPageId.Close()

	var pageId pcgw_integration.PageId
	if err = json.NewDecoder(rcPageId).Decode(&pageId); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.PcgwPageIdProperty:
			values = []string{pageId.GetPageId()}
		}

		piv[property][gogId] = values

	}

	return nil
}
