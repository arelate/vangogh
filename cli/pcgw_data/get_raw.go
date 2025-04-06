package pcgw_data

import (
	"fmt"
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

const (
	txtExt = ".txt"
)

func GetRaw(pcgwGogIds map[string][]string, force bool) error {

	gra := nod.NewProgress("getting %s...", vangogh_integration.PcgwRaw)
	defer gra.Done()

	rawDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwRaw)
	if err != nil {
		return err
	}

	kvRaw, err := kevlar.New(rawDir, txtExt)
	if err != nil {
		return err
	}

	gra.TotalInt(len(pcgwGogIds))

	if err = fetch.Items(maps.Keys(pcgwGogIds), reqs.PcgwRaw(), kvRaw, gra, force); err != nil {
		return err
	}

	return ReduceRaw(pcgwGogIds, kvRaw)
}

func ReduceRaw(pcgwGogIds map[string][]string, kvRaw kevlar.KeyValues) error {

	dataType := vangogh_integration.PcgwRaw

	rra := nod.Begin(" reducing %s...", dataType)
	defer rra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.PcgwRawProperties()...)
	if err != nil {
		return err
	}

	rawReductions := shared_data.InitReductions(vangogh_integration.PcgwRawProperties()...)

	for pcgwPageId, gogIds := range pcgwGogIds {
		if !kvRaw.Has(pcgwPageId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, pcgwPageId))
			continue
		}

		if err = reduceRawProduct(gogIds, pcgwPageId, kvRaw, rawReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, rawReductions)

}

func reduceRawProduct(gogIds []string, pcgwPageId string, kvRaw kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcRaw, err := kvRaw.Get(pcgwPageId)
	if err != nil {
		return err
	}
	defer rcRaw.Close()

	return nil
}
