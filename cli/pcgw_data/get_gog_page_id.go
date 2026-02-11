package pcgw_data

import (
	"encoding/json/v2"
	"errors"
	"maps"

	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
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

	gameGogIds, err := shared_data.GetGameGogIds(gogIds)
	if err != nil {
		return err
	}

	gspia.TotalInt(len(gameGogIds))

	if err = fetch.Items(maps.Keys(gameGogIds), reqs.PcgwGogPageId(), kvGogPageId, gspia, force); err != nil {
		return err
	}

	return ReduceGogPageIds(gameGogIds, kvGogPageId)
}

func ReduceGogPageIds(gogIds map[string]any, kvPageId kevlar.KeyValues) error {

	dataType := vangogh_integration.PcgwGogPageId

	rpia := nod.Begin(" reducing %s...", dataType)
	defer rpia.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.PcgwPageIdProperties()...)
	if err != nil {
		return err
	}

	pageIdReductions := shared_data.InitReductions(vangogh_integration.PcgwPageIdProperties()...)

	for gogId := range gogIds {
		if !kvPageId.Has(gogId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + gogId))
			continue
		}

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
	if err = json.UnmarshalRead(rcPageId, &pageId); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.PcgwPageIdProperty:
			values = []string{pageId.GetPageId()}
		case vangogh_integration.SteamAppIdProperty:
			hasSteamAppId := false
			if steamAppIds, ok := rdx.GetAllValues(vangogh_integration.SteamAppIdProperty, gogId); ok {
				for _, steamAppId := range steamAppIds {
					if steamAppId != "" {
						hasSteamAppId = true
						break
					}
				}
			}
			if hasSteamAppId {
				continue
			}
			values = []string{pageId.GetSteamAppId()}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][gogId] = values
		}

	}

	return nil
}
