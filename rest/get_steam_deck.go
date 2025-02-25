package rest

import (
	"encoding/json"
	"errors"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/http"
)

func GetSteamDeck(w http.ResponseWriter, r *http.Request) {

	// GET /steam-deck?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	gogId := r.URL.Query().Get("id")

	deckCompatibilityReport, err := getDeckAppCompatibilityReport(gogId)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.SteamDeck(gogId, deckCompatibilityReport, rdx)
	if err = p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}

func getDeckAppCompatibilityReport(gogId string) (*steam_integration.DeckAppCompatibilityReport, error) {

	var steamAppId string
	if sai, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok && sai != "" {
		steamAppId = sai
	} else {
		return nil, errors.New("no steam app id for gog id " + gogId)
	}

	deckAppCompatibilityReportDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamDeckCompatibilityReport)
	if err != nil {
		return nil, err
	}

	kvDeckAppCompatibilityReport, err := kevlar.New(deckAppCompatibilityReportDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	rcDeckAppCompatibilityReport, err := kvDeckAppCompatibilityReport.Get(steamAppId)
	if err != nil {
		return nil, err
	}
	defer rcDeckAppCompatibilityReport.Close()

	var deckCompatibilityReport steam_integration.DeckAppCompatibilityReport
	if err = json.NewDecoder(rcDeckAppCompatibilityReport).Decode(&deckCompatibilityReport); err != nil {
		return nil, err
	}

	return &deckCompatibilityReport, nil
}
