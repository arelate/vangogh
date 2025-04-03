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

func GetSteamPageId(steamGogIds map[string][]string, force bool) error {

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

	gspia.TotalInt(len(gameSteamGogIds))

	if err = fetch.Items(maps.Keys(gameSteamGogIds), reqs.PcgwSteamPageId(), kvSteamPageId, gspia, force); err != nil {
		return err
	}

	return ReduceSteamPageIds(gameSteamGogIds, kvSteamPageId)
}

func GetGameSteamGogIds(steamGogIds map[string][]string) (map[string][]string, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.ProductTypeProperty)
	if err != nil {
		return nil, err
	}

	gameSteamGogIds := make(map[string][]string)

	for steamAppId, gogIds := range steamGogIds {
		for _, gogId := range gogIds {
			if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, gogId); ok && pt == "GAME" {
				gameSteamGogIds[steamAppId] = append(gameSteamGogIds[steamAppId], gogId)
			}
		}
	}

	return gameSteamGogIds, nil
}

func ReduceSteamPageIds(steamGogIds map[string][]string, kvPageId kevlar.KeyValues) error {

	dataType := vangogh_integration.PcgwSteamPageId

	rpia := nod.Begin(" reducing %s...", dataType)
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

	for steamAppId, gogIds := range steamGogIds {
		if !kvPageId.Has(steamAppId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, steamAppId))
			continue
		}

		if err = reduceSteamPageIdProduct(gogIds, steamAppId, kvPageId, pageIdReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, pageIdReductions)
}

func reduceSteamPageIdProduct(gogIds []string, steamAppId string, kvPageId kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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

		if shared_data.IsNotEmpty(values...) {
			for _, gogId := range gogIds {
				piv[property][gogId] = values
			}
		}

	}

	return nil
}
