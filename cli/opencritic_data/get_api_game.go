package opencritic_data

import (
	"encoding/json/v2"
	"errors"
	"iter"
	"maps"
	"strconv"

	"github.com/arelate/southern_light/opencritic_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetApiGame(openCriticGogIds map[string][]string, force bool) error {

	gaga := nod.NewProgress("getting %s...", vangogh_integration.OpenCriticApiGame)
	defer gaga.Done()

	apiGameDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.OpenCriticApiGame)

	kvApiGame, err := kevlar.New(apiGameDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gaga.TotalInt(len(openCriticGogIds))

	if err = fetch.Items(maps.Keys(openCriticGogIds), reqs.OpenCriticApiGame(), kvApiGame, gaga, force); err != nil {
		return err
	}

	return ReduceApiGame(maps.Keys(openCriticGogIds), kvApiGame)
}

func ReduceApiGame(openCriticIds iter.Seq[string], kvApiGame kevlar.KeyValues) error {

	dataType := vangogh_integration.OpenCriticApiGame

	raga := nod.Begin(" reducing %s...", dataType)
	defer raga.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.OpenCriticApiGameProperties()...)
	if err != nil {
		return err
	}

	apiGameReductions := shared_data.InitReductions(vangogh_integration.OpenCriticApiGameProperties()...)

	for openCriticId := range openCriticIds {
		if !kvApiGame.Has(openCriticId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + openCriticId))
			continue
		}

		if err = reduceApiGameProduct(openCriticId, kvApiGame, apiGameReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, apiGameReductions)
}

func reduceApiGameProduct(openCriticId string, kvApiGame kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcApiGame, err := kvApiGame.Get(openCriticId)
	if err != nil {
		return err
	}
	defer rcApiGame.Close()

	var apiGame opencritic_integration.ApiGame
	if err = json.UnmarshalRead(rcApiGame, &apiGame); err != nil {
		return nil
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.OpenCriticMedianScoreProperty:
			values = []string{strconv.FormatFloat(apiGame.MedianScore, 'f', -1, 64)}
		case vangogh_integration.OpenCriticTopCriticsScoreProperty:
			values = []string{strconv.FormatFloat(apiGame.TopCriticScore, 'f', -1, 64)}
		case vangogh_integration.OpenCriticPercentileProperty:
			values = []string{strconv.FormatInt(int64(apiGame.Percentile), 10)}
		case vangogh_integration.OpenCriticTierProperty:
			values = []string{apiGame.Tier}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][openCriticId] = values
		}
	}

	return nil
}
