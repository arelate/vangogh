package protondb_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/protondb_integration"
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

func GetSummary(steamGogIds map[string]string, since int64) error {

	gsa := nod.NewProgress("getting %s...", vangogh_integration.ProtonDBSummary)
	defer gsa.Done()

	summaryDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.ProtonDBSummary)
	if err != nil {
		return err
	}

	kvSummary, err := kevlar.New(summaryDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gsa.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.ProtonDbSummary(), kvSummary, gsa); err != nil {
		return err
	}

	return reduceSummary(kvSummary, since)
}

func reduceSummary(kvSummary kevlar.KeyValues, since int64) error {

	rsa := nod.Begin(" reducing %s...", vangogh_integration.ProtonDBSummary)
	defer rsa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.ProtonDbSummaryProperties()...)
	if err != nil {
		return err
	}

	summaryReductions := shared_data.InitReductions(vangogh_integration.ProtonDbSummaryProperties()...)

	updatedSummaries := kvSummary.Since(since, kevlar.Create, kevlar.Update)

	for steamAppId := range updatedSummaries {
		for gogId := range rdx.Match(map[string][]string{vangogh_integration.SteamAppIdProperty: {steamAppId}}, redux.FullMatch) {
			if err = reduceSummaryProduct(gogId, steamAppId, kvSummary, summaryReductions); err != nil {
				return err
			}
		}
	}

	return shared_data.WriteReductions(rdx, summaryReductions)
}

func reduceSummaryProduct(gogId, steamAppId string, kvSummary kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcSummary, err := kvSummary.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcSummary.Close()

	var sum protondb_integration.Summary
	if err = json.NewDecoder(rcSummary).Decode(&sum); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.ProtonDBTierProperty:
			values = []string{sum.String()}
		case vangogh_integration.ProtonDBConfidenceProperty:
			values = []string{sum.GetConfidence()}
		}

		piv[property][gogId] = values

	}

	return nil

}
