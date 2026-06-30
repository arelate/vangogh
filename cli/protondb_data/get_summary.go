package protondb_data

import (
	"encoding/json/v2"
	"errors"
	"iter"
	"maps"

	"github.com/arelate/southern_light/protondb_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetSummary(steamGogIds map[string][]string, since int64, force bool) error {

	gsa := nod.NewProgress("getting %s...", vangogh_integration.ProtonDbSummary)
	defer gsa.Done()

	summaryDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ProtonDbSummary)
	if err != nil {
		return err
	}

	kvSummary, err := kevlar.New(summaryDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gsa.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.ProtonDbSummary(), kvSummary, gsa, force); err != nil {
		return err
	}

	return ReduceSummary(maps.Keys(steamGogIds), kvSummary)
}

func ReduceSummary(steamAppIds iter.Seq[string], kvSummary kevlar.KeyValues) error {

	dataType := vangogh_integration.ProtonDbSummary

	rsa := nod.NewProgress(" reducing %s...", dataType)
	defer rsa.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.ProtonDbSummaryProperties()...)
	if err != nil {
		return err
	}

	summaryReductions := shared_data.InitReductions(vangogh_integration.ProtonDbSummaryProperties()...)

	for steamAppId := range steamAppIds {
		if !kvSummary.Has(steamAppId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + steamAppId))
			rsa.Increment()
			continue
		}

		if err = reduceSummaryProduct(steamAppId, kvSummary, summaryReductions); err != nil {
			return err
		}

		rsa.Increment()
	}

	return shared_data.WriteReductions(rdx, summaryReductions)
}

func reduceSummaryProduct(steamAppId string, kvSummary kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcSummary, err := kvSummary.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcSummary.Close()

	var sum protondb_integration.Summary
	if err = json.UnmarshalRead(rcSummary, &sum); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.ProtonDbTierProperty:
			values = []string{sum.String()}
		case vangogh_integration.ProtonDbConfidenceProperty:
			values = []string{sum.GetConfidence()}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][steamAppId] = values
		}
	}

	return nil

}
