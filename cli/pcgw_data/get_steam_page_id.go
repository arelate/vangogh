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

func GetSteamPageId(steamGogIds map[string]string, force bool) error {

	gspia := nod.NewProgress("getting %s...", vangogh_integration.PcgwSteamPageId)
	defer gspia.Done()

	steamPageIdDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwSteamPageId)
	if err != nil {
		return err
	}

	kvSteamPageId, err := kevlar.New(steamPageIdDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gameSteamGogIds, err := GetGameSteamGogIds(steamGogIds)
	if err != nil {
		return err
	}

	newGameSteamGogIds := GetNewSteamGogIds(gameSteamGogIds, kvSteamPageId, force)

	gspia.TotalInt(len(newGameSteamGogIds))

	if err = fetch.Items(maps.Keys(newGameSteamGogIds), reqs.PcgwSteamPageId(), kvSteamPageId, gspia); err != nil {
		return err
	}

	return ReduceSteamPageIds(newGameSteamGogIds, kvSteamPageId)
}

func GetNewSteamGogIds(steamGogIds map[string]string, kv kevlar.KeyValues, force bool) map[string]string {

	if force {
		return steamGogIds
	}

	newSteamGogIds := make(map[string]string, len(steamGogIds))

	for steamAppId, gogId := range steamGogIds {
		if kv.Has(steamAppId) {
			continue
		}
		newSteamGogIds[steamAppId] = gogId
	}

	return newSteamGogIds
}

func GetGameSteamGogIds(steamGogIds map[string]string) (map[string]string, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.ProductTypeProperty)
	if err != nil {
		return nil, err
	}

	gameSteamGogIds := make(map[string]string)

	for steamAppId, gogId := range steamGogIds {
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, gogId); ok && pt == "GAME" {
			gameSteamGogIds[steamAppId] = gogId
		}
	}

	return gameSteamGogIds, nil
}

func ReduceSteamPageIds(steamGogIds map[string]string, kvPageId kevlar.KeyValues) error {

	rpia := nod.Begin(" reducing %s...", vangogh_integration.PcgwSteamPageId)
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

	for steamAppId, gogId := range steamGogIds {
		if err = reduceSteamPageIdProduct(gogId, steamAppId, kvPageId, pageIdReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, pageIdReductions)
}

func reduceSteamPageIdProduct(gogId, steamAppId string, kvPageId kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcPageId, err := kvPageId.Get(steamAppId)
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
