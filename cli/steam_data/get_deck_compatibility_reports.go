package steam_data

import (
	"encoding/json/v2"
	"errors"
	"iter"
	"maps"
	"strings"

	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetDeckCompatibilityReports(steamGogIds map[string][]string, force bool) error {

	gdcra := nod.NewProgress("getting %s...", vangogh_integration.SteamDeckCompatibilityReport)
	defer gdcra.Done()

	deckCompatibilityReportsDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamDeckCompatibilityReport)

	kvDeckCompatibilityReports, err := kevlar.New(deckCompatibilityReportsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gdcra.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamDeckCompatibilityReports(), kvDeckCompatibilityReports, gdcra, force); err != nil {
		return err
	}

	return ReduceDeckCompatibilityReports(maps.Keys(steamGogIds), kvDeckCompatibilityReports)
}

func ReduceDeckCompatibilityReports(steamAppIds iter.Seq[string], kvDeckCompatibilityReports kevlar.KeyValues) error {

	dataType := vangogh_integration.SteamDeckCompatibilityReport

	rdcra := nod.NewProgress(" reducing %s...", dataType)
	defer rdcra.Done()

	// need to append SteamAppId property to allow mapping to GOG ids
	properties := append(vangogh_integration.SteamDeckCompatibilityReportProperties(), vangogh_integration.GogSteamAppIdProperty)

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), properties...)
	if err != nil {
		return err
	}

	deckCompatibilityReportsReductions := shared_data.InitReductions(vangogh_integration.SteamDeckCompatibilityReportProperties()...)

	for steamAppId := range steamAppIds {
		if !kvDeckCompatibilityReports.Has(steamAppId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + steamAppId))
			rdcra.Increment()
			continue
		}

		if err = reduceDeckCompatibilityReportsProduct(steamAppId, kvDeckCompatibilityReports, deckCompatibilityReportsReductions); err != nil {
			return err
		}

		rdcra.Increment()
	}

	return shared_data.WriteReductions(rdx, deckCompatibilityReportsReductions)
}

func reduceDeckCompatibilityReportsProduct(steamAppId string, kvDeckCompatibilityReports kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcDeckCompatibilityReport, err := kvDeckCompatibilityReports.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcDeckCompatibilityReport.Close()

	var dcr steam_integration.DeckAppCompatibilityReport
	if err = json.UnmarshalRead(rcDeckCompatibilityReport, &dcr); err != nil {
		// handle known empty results that return empty array instead of results
		if strings.Contains(err.Error(), "unmarshal JSON array into Go steam_integration.DeckAppCompatibilityResults") {
			return nil
		}
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
			values = []string{dcr.SteamDeckString()}
		case vangogh_integration.SteamMachineCompatibilityCategoryProperty:
			values = []string{dcr.SteamMachineString()}
		case vangogh_integration.SteamFrameCompatibilityCategoryProperty:
			values = []string{dcr.SteamFrameString()}
		case vangogh_integration.SteamSteamOsAppCompatibilityCategoryProperty:
			values = []string{dcr.SteamOsString()}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][steamAppId] = values
		}

	}

	return nil

}
