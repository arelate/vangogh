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

	gameGogIds, err := GetGameGogIds(gogIds)
	if err != nil {
		return err
	}

	gspia.TotalInt(len(gameGogIds))

	if err = fetch.Items(maps.Keys(gameGogIds), reqs.PcgwGogPageId(), kvGogPageId, gspia, force); err != nil {
		return err
	}

	return ReduceGogPageIds(gameGogIds, kvGogPageId)
}

func GetGameGogIds(gogIds map[string]any) (map[string]any, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.ProductTypeProperty)
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

func ReduceGogPageIds(gogIds map[string]any, kvPageId kevlar.KeyValues) error {

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
		if err = reduceGogPageIdProduct(gogId, kvPageId, pageIdReductions, rdx); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, pageIdReductions)
}

func reduceGogPageIdProduct(gogId string, kvPageId kevlar.KeyValues, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

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
		case vangogh_integration.SteamAppIdProperty:
			if !rdx.HasKey(vangogh_integration.SteamAppIdProperty, gogId) {
				values = []string{pageId.GetSteamAppId()}
			}
		}

		piv[property][gogId] = values

	}

	return nil
}
