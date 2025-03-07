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

func GetEngine(pcgwGogIds map[string]string, force bool) error {

	gea := nod.NewProgress("getting %s...", vangogh_integration.PcgwEngine)
	defer gea.Done()

	engineDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwEngine)
	if err != nil {
		return err
	}

	kvEngine, err := kevlar.New(engineDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	newPcgwGogIds := getNewPcgwGogIds(pcgwGogIds, kvEngine, force)

	gea.TotalInt(len(newPcgwGogIds))

	if err = fetch.Items(maps.Keys(newPcgwGogIds), reqs.PcgwEngine(), kvEngine, gea); err != nil {
		return err
	}

	return ReduceEngine(newPcgwGogIds, kvEngine)
}

func getNewPcgwGogIds(pcgwGogIds map[string]string, kv kevlar.KeyValues, force bool) map[string]string {

	if force {
		return pcgwGogIds
	}

	npgIds := make(map[string]string)

	for pcgwPageId, gogId := range pcgwGogIds {
		if kv.Has(pcgwPageId) {
			continue
		}
		npgIds[pcgwPageId] = gogId
	}

	return npgIds
}

func ReduceEngine(pcgwGogIds map[string]string, kvEngine kevlar.KeyValues) error {

	rea := nod.Begin(" reducing %s...", vangogh_integration.PcgwEngine)
	defer rea.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.PcgwEngineProperties()...)
	if err != nil {
		return err
	}

	engineReductions := shared_data.InitReductions(vangogh_integration.PcgwEngineProperties()...)

	for pcgwPageId, gogId := range pcgwGogIds {
		if err = reduceEngineProduct(gogId, pcgwPageId, kvEngine, engineReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, engineReductions)
}

func reduceEngineProduct(gogId, pcgwPageId string, kvEngine kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcEngine, err := kvEngine.Get(pcgwPageId)
	if err != nil {
		return err
	}
	defer rcEngine.Close()

	var engine pcgw_integration.Engine
	if err = json.NewDecoder(rcEngine).Decode(&engine); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.EnginesProperty:
			values = engine.GetEngines()
		case vangogh_integration.EnginesBuildsProperty:
			values = engine.GetEnginesBuilds()
		}

		piv[property][gogId] = values

	}

	return nil
}
