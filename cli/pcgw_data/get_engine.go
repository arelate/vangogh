package pcgw_data

import (
	"encoding/json"
	"fmt"
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

func GetEngine(pcgwGogIds map[string][]string, force bool) error {

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

	gea.TotalInt(len(pcgwGogIds))

	if err = fetch.Items(maps.Keys(pcgwGogIds), reqs.PcgwEngine(), kvEngine, gea, force); err != nil {
		return err
	}

	return ReduceEngine(pcgwGogIds, kvEngine)
}

func getNewPcgwGogIds(pcgwGogIds map[string][]string, kv kevlar.KeyValues, force bool) map[string][]string {

	if force {
		return pcgwGogIds
	}

	npgIds := make(map[string][]string)

	for pcgwPageId, gogIds := range pcgwGogIds {
		if kv.Has(pcgwPageId) {
			continue
		}
		for _, gogId := range gogIds {
			npgIds[pcgwPageId] = append(npgIds[pcgwPageId], gogId)
		}
	}

	return npgIds
}

func ReduceEngine(pcgwGogIds map[string][]string, kvEngine kevlar.KeyValues) error {

	dataType := vangogh_integration.PcgwEngine

	rea := nod.Begin(" reducing %s...", dataType)
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

	for pcgwPageId, gogIds := range pcgwGogIds {
		if !kvEngine.Has(pcgwPageId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, pcgwPageId))
			continue
		}

		if err = reduceEngineProduct(gogIds, pcgwPageId, kvEngine, engineReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, engineReductions)
}

func reduceEngineProduct(gogIds []string, pcgwPageId string, kvEngine kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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

		if shared_data.IsNotEmpty(values...) {
			for _, gogId := range gogIds {
				piv[property][gogId] = values
			}
		}

	}

	return nil
}
