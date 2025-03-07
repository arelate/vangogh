package steam_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"strings"
)

func GetDeckCompatibilityReports(steamGogIds map[string]string, since int64) error {

	gdcra := nod.NewProgress("getting %s...", vangogh_integration.SteamDeckCompatibilityReport)
	defer gdcra.Done()

	deckCompatibilityReportsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamDeckCompatibilityReport)
	if err != nil {
		return err
	}

	kvDeckCompatibilityReports, err := kevlar.New(deckCompatibilityReportsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gdcra.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamDeckCompatibilityReports(), kvDeckCompatibilityReports, gdcra); err != nil {
		return err
	}

	return ReduceDeckCompatibilityReports(kvDeckCompatibilityReports, since)
}

func ReduceDeckCompatibilityReports(kvDeckCompatibilityReports kevlar.KeyValues, since int64) error {

	rdcra := nod.Begin(" reducing %s...", vangogh_integration.SteamDeckCompatibilityReport)
	defer rdcra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	// need to append SteamAppId property to allow mapping to GOG ids
	properties := append(vangogh_integration.SteamDeckCompatibilityReportProperties(), vangogh_integration.SteamAppIdProperty)

	rdx, err := redux.NewWriter(reduxDir, properties...)
	if err != nil {
		return err
	}

	deckCompatibilityReportsReductions := shared_data.InitReductions(vangogh_integration.SteamDeckCompatibilityReportProperties()...)

	updatedDeckCompatibilityReviews := kvDeckCompatibilityReports.Since(since, kevlar.Create, kevlar.Update)

	for steamAppId := range updatedDeckCompatibilityReviews {
		for gogId := range rdx.Match(map[string][]string{vangogh_integration.SteamAppIdProperty: {steamAppId}}, redux.FullMatch) {
			if err = reduceDeckCompatibilityReportsProduct(gogId, steamAppId, kvDeckCompatibilityReports, deckCompatibilityReportsReductions); err != nil {
				return err
			}
		}
	}

	return shared_data.WriteReductions(rdx, deckCompatibilityReportsReductions)
}

func reduceDeckCompatibilityReportsProduct(gogId, steamAppId string, kvDeckCompatibilityReports kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcDeckCompatibilityReport, err := kvDeckCompatibilityReports.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcDeckCompatibilityReport.Close()

	var dcr steam_integration.DeckAppCompatibilityReport
	if err = json.NewDecoder(rcDeckCompatibilityReport).Decode(&dcr); err != nil {
		// handle known empty results that return empty array instead of results
		if strings.Contains(err.Error(), "cannot unmarshal array into Go struct field DeckAppCompatibilityReport.results") {
			return nil
		}
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
			values = []string{dcr.String()}
		}

		piv[property][gogId] = values

	}

	return nil

}
